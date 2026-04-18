import { BasePage } from './base-page.js';

export class ProfilePage extends BasePage {
    get path() {
        return '/profile';
    }

    async openDirect() {
        await this.page.goto('/profile');
    }
}