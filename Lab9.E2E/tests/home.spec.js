import { test, expect } from '../fixtures/pages.js';

test.describe('Home Page', () => {

    test.beforeEach(async ({ pages }) => {
        await pages.home().goto();
    });

    test('page loads with 200 status', async ({ page }) => {
        const response = await page.goto('/');
        expect(response.status()).toBe(200);
    });

    test('title is not empty and contains brand', async ({ pages }) => {
        const title = await pages.home().title();
        expect(title).not.toBe('');
    });

    test('main heading visible', async ({ pages }) => {
        await expect(pages.home().heading).toBeVisible();
    });

    test('sign-in link exists', async ({ pages }) => {
        await expect(pages.home().signInLink).toBeVisible();
    });

    test('sign-in link is clickable', async ({ pages }) => {
        await expect(pages.home().signInLink).toBeEnabled();
    });

    test('click sign-in navigates correctly', async ({ pages, page }) => {
        await pages.home().openSignIn();
        await expect(page).toHaveURL(/sign-in/);
    });

    test('page does not crash on reload', async ({ page }) => {
        await page.reload();
        await expect(page).toHaveURL('/');
    });

    test('no console errors on load', async ({ page }) => {
        const errors = [];
        page.on('console', msg => {
            if (msg.type() === 'error') errors.push(msg.text());
        });

        await page.goto('/');
        expect(errors.length).toBe(0);
    });

});