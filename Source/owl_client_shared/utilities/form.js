class Form
{
    constructor() {
        this.loading = false;
        this.leaving = false;
        this.error = null;
        this.errors = {};

        this.fields = {};
        this.validations = {};
    }

    clearErrors() {
        this.errors = {};
        this.error = null;
    }

    setError(error) {
        this.clearErrors();
        this.error = error;
    }

    setLoading(toggle) {
        this.loading = toggle;
    }

    setLeaving(toggle) {
        this.leaving = toggle;
    }

    validate() {
        this.clearErrors();
        let validator = new Validator(this.fields, this.validations);

        this.errors = validator.validate();

        if (this.isValid()) {
            return true;
        }

        this.error = "Some fields failed validation. Please correct them and try again!";
        return false;
    }

    errorCount() {
        return Object.keys(this.errors).length;
    }

    isValid() {
        return this.errorCount() === 0;
    }
}