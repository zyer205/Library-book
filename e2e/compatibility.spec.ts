import { test, expect } from '@playwright/test';

test.describe('Mobile viewport compatibility (375x812)', () => {
  test.use({ viewport: { width: 375, height: 812 } });

  test('Home page renders on mobile', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('body')).toBeVisible();
    // Cards should stack vertically on mobile
    const cards = page.locator('.card');
    const count = await cards.count();
    expect(count).toBeGreaterThan(0);
  });

  test('Seat list renders on mobile', async ({ page }) => {
    await page.goto('/');
    await page.locator('form button[type="submit"]').first().click();
    await expect(page).toHaveURL(/\/Seats/);
    // Seat cards should be in 2-column grid on mobile
    await expect(page.locator('.seat-card').first()).toBeVisible();
  });

  test('Admin login page renders on mobile', async ({ page }) => {
    await page.goto('/Admin/Login');
    await expect(page.locator('h3')).toContainText('管理员登录');
    // Login form should be centered and readable
    await expect(page.locator('input[name="Username"]')).toBeVisible();
  });
});

test.describe('Desktop compatibility (1280x720)', () => {
  test.use({ viewport: { width: 1280, height: 720 } });

  test('Admin seats table renders on desktop', async ({ page }) => {
    await page.goto('/Admin/Login');
    await page.fill('input[name="Username"]', 'admin');
    await page.fill('input[name="Password"]', 'admin123');
    await page.locator('button[type="submit"]').click();
    await expect(page).toHaveURL(/\/Admin\/Reservations/);
    await page.goto('/Admin/Seats');
    // Desktop should show sidebar
    await expect(page.locator('.admin-sidebar')).toBeVisible();
    await expect(page.locator('h2')).toContainText('座位管理');
  });

  test('Admin statistics card grid on desktop', async ({ page }) => {
    await page.goto('/Admin/Login');
    await page.fill('input[name="Username"]', 'admin');
    await page.fill('input[name="Password"]', 'admin123');
    await page.locator('button[type="submit"]').click();
    await expect(page).toHaveURL(/\/Admin\/Reservations/);
    await page.goto('/Admin/Statistics');
    await expect(page.locator('h2')).toContainText('统计');
    // Should see KPI card layout
    await expect(page.locator('.card').first()).toBeVisible();
  });
});
