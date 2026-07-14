import { test, expect } from '@playwright/test';

test('Home page loads', async ({ page }) => {
  await page.goto('/');
  await expect(page.locator('body')).toBeVisible();
  const title = await page.title();
  console.log('Page title:', title);
});

test('Admin login page renders', async ({ page }) => {
  await page.goto('/Admin/Login');
  await expect(page.locator('h3')).toContainText('管理员登录');
});

test('Switch identity and see seats', async ({ page }) => {
  await page.goto('/');
  // Click first user card form button
  await page.locator('form button[type="submit"]').first().click();
  await expect(page).toHaveURL(/\/Seats/);
  // Verify seats rendered
  await expect(page.locator('.container')).toBeVisible();
});
