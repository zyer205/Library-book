import { test, expect } from '@playwright/test';

test.describe('User-side smoke', () => {
  test('Home page loads with identity cards', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('body')).toBeVisible();
    await expect(page.locator('form button[type=\"submit\"]').first()).toBeVisible();
  });

  test('Switch identity and view seats', async ({ page }) => {
    await page.goto('/');
    await page.locator('form button[type=\"submit\"]').first().click();
    await expect(page).toHaveURL(/\/Seats/);
  });

  test('View seat detail page', async ({ page }) => {
    await page.goto('/');
    await page.locator('form button[type=\"submit\"]').first().click();
    await expect(page).toHaveURL(/\/Seats/);
    await page.goto('/Seats/Detail/1');
    await expect(page.locator('body')).toBeVisible();
  });

  test('Reservation create page accessible', async ({ page }) => {
    await page.goto('/');
    await page.locator('form button[type=\"submit\"]').first().click();
    await expect(page).toHaveURL(/\/Seats/);
    await page.goto('/Reservation/Create?seatId=6');
    await expect(page.locator('body')).toBeVisible();
  });

  test('My reservations page accessible', async ({ page }) => {
    await page.goto('/');
    await page.locator('form button[type=\"submit\"]').first().click();
    await expect(page).toHaveURL(/\/Seats/);
    await page.goto('/Reservation/My');
    await expect(page.locator('body')).toBeVisible();
  });
});
