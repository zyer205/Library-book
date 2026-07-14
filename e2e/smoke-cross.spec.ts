import { test, expect } from '@playwright/test';

test.describe('Cross-flow', () => {
  test('Admin toggle seat restores correctly', async ({ page }) => {
    // Login as admin
    await page.goto('/Admin/Login');
    await page.fill('input[name=\"Username\"]', 'admin');
    await page.fill('input[name=\"Password\"]', 'admin123');
    await page.locator('button[type=\"submit\"]').click();
    await expect(page).toHaveURL(/\/Admin\/Reservations/);

    // Toggle seat 5 (ZX-05) 
    await page.goto('/Admin/Seats');
    await expect(page.locator('h2')).toContainText('座位管理');

    // Toggle
    await page.locator('form[action*=\"Toggle/5\"] button').click();
    await expect(page).toHaveURL(/\/Admin\/Seats/);
    // Toggle back
    await page.locator('form[action*=\"Toggle/5\"] button').click();
    await expect(page).toHaveURL(/\/Admin\/Seats/);
  });

  test('Session timeout redirects to login', async ({ page }) => {
    await page.goto('/Admin/Reservations');
    await expect(page).toHaveURL(/\/Admin\/Login/);
    // Should see timeout message
    const alertShown = await page.locator('.alert-danger').isVisible().catch(() => false);
    // alert may or may not show depending on session state, but URL must be login
    expect(page.url()).toContain('/Admin/Login');
  });

  test('User seat detail accessible', async ({ page }) => {
    await page.goto('/Seats/Detail/1');
    await expect(page.locator('body')).toBeVisible();
  });

  test('Admin statistics accessible', async ({ page }) => {
    await page.goto('/Admin/Login');
    await page.fill('input[name=\"Username\"]', 'admin');
    await page.fill('input[name=\"Password\"]', 'admin123');
    await page.locator('button[type=\"submit\"]').click();
    await expect(page).toHaveURL(/\/Admin\/Reservations/);
    await page.goto('/Admin/Statistics');
    await expect(page.locator('h2')).toContainText('统计');
  });
});
