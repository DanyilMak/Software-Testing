import { test, expect } from '../fixtures/pages.js';

test.describe('Protected routes (no auth)', () => {

    test('profile redirects to sign-in', async ({ page }) => {
        await page.goto('/profile');
        await expect(page).toHaveURL(/sign-in/);
    });

    test('dashboard redirects to sign-in', async ({ page }) => {
        await page.goto('/dashboard');
        await expect(page).toHaveURL(/sign-in/);
    });

    test('settings redirects to sign-in', async ({ page }) => {
        await page.goto('/settings');
        await expect(page).toHaveURL(/sign-in/);
    });

    test('random private route redirects', async ({ page }) => {
        await page.goto('/admin');
        await expect(page).toHaveURL(/sign-in/);
    });

    test('redirect happens quickly', async ({ page }) => {
        const start = Date.now();

        await page.goto('/profile');
        await expect(page).toHaveURL(/sign-in/);

        const duration = Date.now() - start;
        expect(duration).toBeLessThan(3000);
    });

    test('no access without session', async ({ context, page }) => {
        await context.clearCookies();

        await page.goto('/profile');

        await expect(page).toHaveURL(/sign-in/);
    });

});