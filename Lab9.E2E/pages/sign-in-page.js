import { BasePage } from './base-page.js';

export class SignInPage extends BasePage {
    get path() {
        return '/sign-in';
    }

    get googleButton() {
        return this.page.getByRole('button', { name: /google/i });
    }

    get microsoftButton() {
        return this.page.getByRole('button', { name: /microsoft/i });
    }

    get heading() {
        return this.page.getByRole('heading');
    }
}