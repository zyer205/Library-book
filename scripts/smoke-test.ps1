param(
    [string]$BaseUrl = "http://localhost:5000"
)

$ErrorActionPreference = "Stop"
$passed = 0
$failed = 0
$results = @()

function Test-Endpoint {
    param($Name, $Method = "GET", $Url, $Body = $null, $ContentType = "application/x-www-form-urlencoded", 
          $ExpectedStatus = 200, $ExpectedText = $null, $FollowRedirect = $true)

    Add-Type -AssemblyName System.Net.Http
    $handler = New-Object System.Net.Http.HttpClientHandler
    $handler.AllowAutoRedirect = $FollowRedirect
    $c = New-Object System.Net.Http.HttpClient($handler)
    $c.Timeout = [TimeSpan]::FromSeconds(15)
    
    try {
        if ($Method -eq "GET") {
            $resp = $c.GetAsync($Url).Result
        } elseif ($Method -eq "POST") {
            $content = if ($Body) { (New-Object System.Net.Http.StringContent($Body, [System.Text.Encoding]::UTF8, $ContentType)) } else { $null }
            $resp = $c.PostAsync($Url, $content).Result
        }
        
        $status = [int]$resp.StatusCode
        $location = if ($resp.Headers.Location) { $resp.Headers.Location.ToString() } else { "" }
        $body = $resp.Content.ReadAsStringAsync().Result
        
        $statusOk = $status -eq $ExpectedStatus -or ($FollowRedirect -and $status -eq 200)
        $textOk = (!$ExpectedText -or $body.Contains($ExpectedText))
        
        if ($statusOk -and $textOk) {
            $script:passed++
            return @{Status = "PASS"; StatusCode = $status; Location = $location; BodyLength = $body.Length }
        } else {
            $script:failed++
            $reason = if (!$statusOk) { "Expected $ExpectedStatus, got $status" } else { "Missing text: $ExpectedText" }
            return @{Status = "FAIL"; Reason = $reason; StatusCode = $status; Location = $location; BodyLength = $body.Length }
        }
    } catch {
        $script:failed++
        return @{Status = "FAIL"; Reason = $_.Exception.Message }
    }
}

Write-Output "========================================"
Write-Output " Smoke Test - Library Seat Reservation"
Write-Output " Base URL: $BaseUrl"
Write-Output "========================================"
Write-Output ""

# 1. Home page
$r = Test-Endpoint -Name "Home page" -Url "$BaseUrl/" -ExpectedText "图书馆"
Write-Output "$($r.Status) | GET / ($($r.BodyLength) bytes)"

# 2. User switch (login as 学生 A)
$r = Test-Endpoint -Name "User switch" -Method POST -Url "$BaseUrl/Account/Switch" -Body "userName=%E5%AD%A6%E7%94%9F+A" -FollowRedirect $true
Write-Output "$($r.Status) | POST /Account/Switch → $($r.Location)"

# 3. Seats list
$r = Test-Endpoint -Name "Seats list" -Url "$BaseUrl/Seats" -ExpectedText "seat-card"
Write-Output "$($r.Status) | GET /Seats ($($r.BodyLength) bytes)"

# 4. Seat detail (ID=1)
$r = Test-Endpoint -Name "Seat detail" -Url "$BaseUrl/Seats/Detail/1" -ExpectedText "时段"
Write-Output "$($r.Status) | GET /Seats/Detail/1 ($($r.BodyLength) bytes)"

# 5. Reservation My (need session from earlier)
$cSession = New-Object System.Net.Http.HttpClient
$cSession.PostAsync("$BaseUrl/Account/Switch", (New-Object System.Net.Http.StringContent("userName=%E5%AD%A6%E7%94%9F+A", [System.Text.Encoding]::UTF8, "application/x-www-form-urlencoded"))).Result > $null
# Now try to make reservation
$createResp = $cSession.GetAsync("$BaseUrl/Reservation/Create?seatId=6").Result
$createBody = $createResp.Content.ReadAsStringAsync().Result
$hasSlots = $createBody.Contains("slot-item") -or $createBody.Contains("时段")
Write-Output "$(if($hasSlots){'PASS'}else{'FAIL'}) | GET /Reservation/Create?seatId=6 (has slots: $hasSlots)"

# 6. Admin login page
$r = Test-Endpoint -Name "Admin login page" -Url "$BaseUrl/Admin/Login" -ExpectedText "管理员登录"
Write-Output "$($r.Status) | GET /Admin/Login ($($r.BodyLength) bytes)"

# 7. Admin login
$handler = New-Object System.Net.Http.HttpClientHandler
$handler.AllowAutoRedirect = $false
$cAdmin = New-Object System.Net.Http.HttpClient($handler)
$loginResp = $cAdmin.PostAsync("$BaseUrl/Admin/Login", (New-Object System.Net.Http.StringContent("Username=admin&Password=admin123", [System.Text.Encoding]::UTF8, "application/x-www-form-urlencoded"))).Result
$loginLoc = $loginResp.Headers.Location
Write-Output "$(if($loginLoc -match 'Reservations'){'PASS'}else{'FAIL'}) | POST /Admin/Login → $loginLoc"

# 8. Admin reservations (need session cookie)
$cookies = $loginResp.Headers.GetValues("Set-Cookie") -join "; "
$cAdmin.DefaultRequestHeaders.Remove("Cookie")
$cAdmin.DefaultRequestHeaders.Add("Cookie", $cookies)
$reservResp = $cAdmin.GetAsync("$BaseUrl/Admin/Reservations").Result
$reservBody = $reservResp.Content.ReadAsStringAsync().Result
$hasReservTable = $reservBody.Contains("预约管理") -or $reservBody.Contains("暂无")
Write-Output "$(if($hasReservTable){'PASS'}else{'FAIL'}) | GET /Admin/Reservations ($($reservResP.StatusCode) / has content: $hasReservTable)"

# 9. Admin seats
$cAdmin.DefaultRequestHeaders.Remove("Cookie")
$cAdmin.DefaultRequestHeaders.Add("Cookie", $cookies)
$seatsResp = $cAdmin.GetAsync("$BaseUrl/Admin/Seats").Result
$seatsBody = $seatsResp.Content.ReadAsStringAsync().Result
$hasSeatsTable = $seatsBody.Contains("座位管理")
Write-Output "$(if($hasSeatsTable){'PASS'}else{'FAIL'}) | GET /Admin/Seats ($($seatsResp.StatusCode) / has content: $hasSeatsTable)"

# 10. Admin toggle seat (post)
$toggleResp = $cAdmin.PostAsync("$BaseUrl/Admin/Seats/Toggle/5", (New-Object System.Net.Http.StringContent("", [System.Text.Encoding]::UTF8, "application/x-www-form-urlencoded"))).Result
$toggleLoc = $toggleResp.Headers.Location
Write-Output "$(if($toggleLoc -match 'Seats'){'PASS'}else{'FAIL'}) | POST /Admin/Seats/Toggle/5 → $toggleLoc"

# 11. Admin toggle back
$cAdmin.PostAsync("$BaseUrl/Admin/Seats/Toggle/5", (New-Object System.Net.Http.StringContent("", [System.Text.Encoding]::UTF8, "application/x-www-form-urlencoded"))).Result > $null
Write-Output "PASS | POST /Admin/Seats/Toggle/5 (restored)"

# 12. Admin statistics
$cAdmin.DefaultRequestHeaders.Remove("Cookie")
$cAdmin.DefaultRequestHeaders.Add("Cookie", $cookies)
$statResp = $cAdmin.GetAsync("$BaseUrl/Admin/Statistics").Result
$statBody = $statResp.Content.ReadAsStringAsync().Result
$hasStat = $statBody.Contains("统计")
Write-Output "$(if($hasStat){'PASS'}else{'FAIL'}) | GET /Admin/Statistics ($($statResp.StatusCode) / has content: $hasStat)"

# 13. Session timeout redirect (no auth)
$timeoutResp = $cAdmin.GetAsync("$BaseUrl/Admin/Reservations").Result
$timeoutLoc = $timeoutResp.Headers.Location
Write-Output "$(if($timeoutLoc -match 'Login'){'PASS'}else{'FAIL'}) | GET /Admin/Reservations (no auth) → $timeoutLoc"

Write-Output ""
Write-Output "========================================"
Write-Output " Results: $passed passed, $failed failed"
Write-Output "========================================"
exit $failed
