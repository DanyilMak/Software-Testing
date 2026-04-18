import { HomePage } from './home-page.js';
import { SignInPage } from './sign-in-page.js';
import { ProfilePage } from './profile-page.js';

export class PageFactory {
    constructor(page) {
        this.page = page;
    }

    home() {
        return new HomePage(this.page);
    }

    signIn() {
        return new SignInPage(this.page);
    }

    profile() {
        return new ProfilePage(this.page);
    }
}