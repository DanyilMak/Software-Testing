import { test as base } from '@playwright/test';
import { PageFactory } from '../pages/page-factory.js';

export const test = base.extend({
    pages: async ({ page }, use) => {
        await use(new PageFactory(page));
    }
});

export { expect } from '@playwright/test';