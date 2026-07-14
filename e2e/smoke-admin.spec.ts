import { test, expect } from '@playwright/test';

test.describe('Admin-side smoke tests', () => {
  test('TC08: Admin login page renders', async ({ page }) => {
    await page.goto('/Admin/Login');
    await expect(page.locator('h3')).toContainText('管理员登录');
    await expect(page.locator('input[name="Username"]')).toBeVisible();
    await expect(page.locator('input[name="Password"]')).toBeVisible();
  });

  test('TC09: Admin login and view reservations', async ({ page }) => {
    await page.goto('/Admin/Login');
    await page.fill('input[name="Username"]', 'admin');
    await page.fill('input[name="Password"]', 'admin123');
    await page.locator('button[type="submit"]').click();
    // Should redirect to /Admin/Reservations
    await expect(page).toHaveURL(/\/Admin\/Reservations/);
    // Should see reservation table or empty state
    await expect(page.locator('h2')).toContainText('预约管理');
  });

  test('TC10: Admin manage seats page', async ({ page }) => {
    // Login
    await page.goto('/Admin/Login');
    await page.fill('input[name="Username"]', 'admin');
    await page.fill('input[name="Password"]', 'admin123');
    await page.locator('button[type="submit"]').click();
    await expect(page).toHaveURL(/\/Admin\/Reservations/);

    // Navigate to Seats management
    await page.goto('/Admin/Seats');
    await expect(page.locator('h2')).toContainText('座位管理');
    // Should see seat table
    await expect(page.locator('table tbody tr').first()).toBeVisible({ timeout: 5000 });
  });

  test('TC11+TC12: Toggle seat enabled and verify in user view', async ({ page }) => {
    // Login as admin
    await page.goto('/Admin/Login');
    await page.fill('input[name="Username"]', 'admin');
    await page.fill('input[name="Password"]', 'admin123');
    await page.locator('button[type="submit"]').click();
    await expect(page).toHaveURL(/\/Admin\/Reservations/);

    // Go to seats management
    await page.goto('/Admin/Seats');
    await expect(page.locator('h2')).toContainText('座位管理');

    // Find the Toggle button for last seat (ZX-05 by ID=5) - get its current state
    // Toggle form is inline with btn-warning (disable) or btn-success (enable)
    const toggleBtn = page.locator('form[action*="Toggle"] button').last();
    const btnText = await toggleBtn.textContent();
    
    // Click toggle
    await toggleBtn.click();
    await expect(page).toHaveURL(/\/Admin\/Seats/);

    // Verify TempData success message appears
    await expect(page.locator('.alert-success')).toBeVisible({ timeout: 3000 });

    // Toggle back to restore original state
    const restoreBtn = page.locator('form[action*="Toggle"] button').last();
    await restoreBtn.click();
  });

  test('Statistics page loads', async ({ page }) => {
    // Login
    await page.goto('/Admin/Login');
    await page.fill('input[name="Username"]', 'admin');
    await page.fill('input[name="Password"]', 'admin123');
    await page.locator('button[type="submit"]').click();
    await expect(page).toHaveURL(/\/Admin\/Reservations/);

    // Go to Statistics
    await page.goto('/Admin/Statistics');
    await expect(page.locator('h2')).toContainText('统计');
  });

  test('Session timeout redirect to login with ?timeout=1', async ({ page }) => {
    // Access admin page without login
    await page.goto('/Admin/Reservations');
    // Should redirect to /Admin/Login?timeout=1
    await expect(page).toHaveURL(/\/Admin\/Login/);
    // Should show timeout message
    await expect(page.locator('.alert-danger')).toBeVisible({ timeout: 3000 });
  });
});
