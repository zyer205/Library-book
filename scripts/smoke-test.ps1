param([string]$BaseUrl = "http://localhost:5000")

$ErrorActionPreference = "Stop"

function Test-Endpoint {
    param($Method = "GET", $Url, $Body = $null, $ContentType = "application/x-www-form-urlencoded",
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
        $ok = $statusOk -and $textOk
        $reason = if (!$statusOk) { "Expected $ExpectedStatus, got $status" } elseif (!$textOk) { "Missing text: $ExpectedText" } else { "" }

        return @{ Ok = $ok; Status = if($ok){"PASS"}else{"FAIL"}; Reason = $reason; StatusCode = $status; Location = $location; BodyLength = $body.Length }
    } catch {
        return @{ Ok = $false; Status = "FAIL"; Reason = $_.Exception.Message; StatusCode = 0; Location = ""; BodyLength = 0 }
    }
}

Write-Output "========================================"
Write-Output " Smoke Test - Library Seat Reservation"
Write-Output " Base URL: $BaseUrl"
Write-Output "========================================"
Write-Output ""

$passed = 0
$failed = 0

function Check {
    param($Result, $Label)
    if ($Result.Ok) { $script:passed++ } else { $script:failed++ }
    $loc = if ($Result.Location) { " → $($Result.Location)" } else { "" }
    $reason = if ($Result.Reason) { " [$($Result.Reason)]" } else { "" }
    Write-Output "$($Result.Status) | $Label ($($Result.BodyLength) bytes$loc$reason)"
}

# 1. Home page
Check (Test-Endpoint -Url "$BaseUrl/" -ExpectedText "图书馆") "GET /"

# 2. User switch (login as 学生 A)
Check (Test-Endpoint -Method POST -Url "$BaseUrl/Account/Switch" -Body "userName=%E5%AD%A6%E7%94%9F+A") "POST /Account/Switch"

# 3. Seats list
Check (Test-Endpoint -Url "$BaseUrl/Seats") "GET /Seats"

# 4. Seat detail (ID=1)
Check (Test-Endpoint -Url "$BaseUrl/Seats/Detail/1") "GET /Seats/Detail/1"

# 5. Reservation Create page
Check (Test-Endpoint -Url "$BaseUrl/Reservation/Create?seatId=6") "GET /Reservation/Create?seatId=6"

# 6. Admin login page
Check (Test-Endpoint -Url "$BaseUrl/Admin/Login" -ExpectedText "管理员登录") "GET /Admin/Login"

# 7. Admin login → should redirect to /Admin/Reservations
Check (Test-Endpoint -Method POST -Url "$BaseUrl/Admin/Login" -Body "Username=admin&Password=admin123" -FollowRedirect $false -ExpectedStatus 302) "POST /Admin/Login"

# 8. Login + follow redirect, check reservation page
$handlerL = New-Object System.Net.Http.HttpClientHandler
$handlerL.AllowAutoRedirect = $true
$cL = New-Object System.Net.Http.HttpClient($handlerL)
$loginResp = $cL.PostAsync("$BaseUrl/Admin/Login", (New-Object System.Net.Http.StringContent("Username=admin&Password=admin123", [System.Text.Encoding]::UTF8, "application/x-www-form-urlencoded"))).Result
$reservResp = $cL.GetAsync("$BaseUrl/Admin/Reservations").Result
$reservBody = $reservResp.Content.ReadAsStringAsync().Result
$reservOk = $reservBody.Contains("预约管理") -or $reservBody.Contains("暂无")
if ($reservOk) { $passed++ } else { $failed++ }
Write-Output "$(if($reservOk){'PASS'}else{'FAIL'}) | GET /Admin/Reservations (authenticated)"

# 9. Admin seats
$seatsResp = $cL.GetAsync("$BaseUrl/Admin/Seats").Result
$seatsBody = $seatsResp.Content.ReadAsStringAsync().Result
$seatsOk = $seatsBody.Contains("座位管理")
if ($seatsOk) { $passed++ } else { $failed++ }
Write-Output "$(if($seatsOk){'PASS'}else{'FAIL'}) | GET /Admin/Seats (authenticated)"

# 10. Admin toggle seat
$handlerT = New-Object System.Net.Http.HttpClientHandler
$handlerT.AllowAutoRedirect = $false
$cT = New-Object System.Net.Http.HttpClient($handlerT)
# Need to re-login for this client
$cT.PostAsync("$BaseUrl/Admin/Login", (New-Object System.Net.Http.StringContent("Username=admin&Password=admin123", [System.Text.Encoding]::UTF8, "application/x-www-form-urlencoded"))).Result > $null
$toggleResp = $cT.PostAsync("$BaseUrl/Admin/Seats/Toggle/5", (New-Object System.Net.Http.StringContent("", [System.Text.Encoding]::UTF8, "application/x-www-form-urlencoded"))).Result
$toggleLoc = if ($toggleResp.Headers.Location) { $toggleResp.Headers.Location.ToString() } else { "" }
$toggleOk = $toggleLoc -match 'Seats'
$cT.PostAsync("$BaseUrl/Admin/Seats/Toggle/5", (New-Object System.Net.Http.StringContent("", [System.Text.Encoding]::UTF8, "application/x-www-form-urlencoded"))).Result > $null  # restore
if ($toggleOk) { $passed++ } else { $failed++ }
Write-Output "$(if($toggleOk){'PASS'}else{'FAIL'}) | POST /Admin/Seats/Toggle/5 → $toggleLoc"

# 11. Admin statistics
$statResp = $cL.GetAsync("$BaseUrl/Admin/Statistics").Result
$statBody = $statResp.Content.ReadAsStringAsync().Result
$statOk = $statBody.Contains("统计")
if ($statOk) { $passed++ } else { $failed++ }
Write-Output "$(if($statOk){'PASS'}else{'FAIL'}) | GET /Admin/Statistics (authenticated)"

# 12. Session timeout redirect (fresh HttpClient — no cookies)
$freshHandler = New-Object System.Net.Http.HttpClientHandler
$freshHandler.AllowAutoRedirect = $false
$freshClient = New-Object System.Net.Http.HttpClient($freshHandler)
$timeoutResp = $freshClient.GetAsync("$BaseUrl/Admin/Reservations").Result
$timeoutLoc = if ($timeoutResp.Headers.Location) { $timeoutResp.Headers.Location.ToString() } else { "" }
$timeoutOk = $timeoutLoc -match 'Login'
if ($timeoutOk) { $passed++ } else { $failed++ }
Write-Output "$(if($timeoutOk){'PASS'}else{'FAIL'}) | GET /Admin/Reservations (no auth) → $timeoutLoc"

Write-Output ""
Write-Output "========================================"
Write-Output " Results: $passed passed, $failed failed"
Write-Output "========================================"
exit $failed
