import { test, expect } from '../fixtures/pages.js';

test.describe('Sign In Page', () => {

  test.beforeEach(async ({ pages }) => {
    await pages.signIn().goto();
  });

  test('page loads', async ({ page }) => {
    await expect(page).toHaveURL(/sign-in/);
  });

  test('heading visible', async ({ pages }) => {
    await expect(pages.signIn().heading).toBeVisible();
  });

  test('google button visible', async ({ pages }) => {
    await expect(pages.signIn().googleButton).toBeVisible();
  });

  test('microsoft button visible', async ({ pages }) => {
    await expect(pages.signIn().microsoftButton).toBeVisible();
  });

  test('google button clickable', async ({ pages }) => {
    await expect(pages.signIn().googleButton).toBeEnabled();
  });

  test('microsoft button clickable', async ({ pages }) => {
    await expect(pages.signIn().microsoftButton).toBeEnabled();
  });

  test('google click redirects to OAuth', async ({ pages, page }) => {
    test.skip(process.env.CI, 'Skip OAuth in CI');
    await pages.signIn().googleButton.click();

    await expect(page).toHaveURL(/accounts.google.com|oauth/i);
  });

  test('microsoft click redirects to OAuth', async ({ pages, page }) => {
    test.skip(process.env.CI, 'Skip OAuth in CI');
    await pages.signIn().microsoftButton.click();

    await expect(page).toHaveURL(/microsoftonline.com|oauth/i);
  });

});