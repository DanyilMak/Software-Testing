import { BasePage } from './base-page.js';

export class HomePage extends BasePage {
    get path() {
        return '/';
    }

    get signInLink() {
        return this.page.getByRole('link', {
            name: /sign|увійти/i
        });
    }

    get heading() {
        return this.page.getByRole('heading').first();
    }

    async openSignIn() {
        await this.signInLink.click();
    }
}