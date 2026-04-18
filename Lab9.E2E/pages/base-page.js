export class BasePage {
    constructor(page) {
        this.page = page;
    }

    get path() {
        return '/';
    }

    async goto() {
        await this.page.goto(this.path, {
            waitUntil: 'networkidle'
        });
    }

    async title() {
        return await this.page.title();
    }
}