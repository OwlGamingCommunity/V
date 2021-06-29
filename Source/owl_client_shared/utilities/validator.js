class Validator
{
    constructor(data, validations) {
        this.data = data;
        this.validations = validations;
        this.errors = {};
        this.messages = {
            min: ':field must be at least :arg0 characters long.',
            max: ':field must be less than :arg0 characters long.',
            email: ':field must be a valid email.',
            repeated: ":field must match :arg0.",
            length: ":field must be :arg0 characters long.",
            number: ":field must be a number.",
            string: ":field must be a string.",
            boolean: ":field must be a boolean.",
            required: ":field is required.",
            lessThanOrEqual: ":field must be less than or equal to :arg0",
            greaterThanOrEqual: ":field must be greater than or equal to :arg0",
        }
    }

    validate() {
        for (const [key, value] of Object.entries(this.data)) {
            let validators = this.validations[key];
            if (validators === undefined) {
                continue;
            }

            for (let validationString of validators) {
                let parts = validationString.split(':');
                let validator = parts.shift();

                let args = [];
                if (parts.length > 0) {
                    args = parts.shift().split(',');
                }

                let method = `validate${StringUtils.capitalizeFirstLetter(validator)}`
                let isValid = this[method](value, ...args);

                if (!isValid) {
                    this.addError(key, validator, args);
                    break;
                }
            }
        }

        return this.errors;
    }

    getFormattedMessage(message, field, args) {
        var result = field.replace( /([A-Z])/g, " $1" );
        var formattedField = result.charAt(0).toUpperCase() + result.slice(1);

        message = message.replace(':field', formattedField);

        for (const key in args) {
            let argValue = args[key];
            message = message.replace(':arg' + key, argValue);
        }

        return message;
    }

    addError(field, validator, args) {
        let message = this.messages[validator];
        message = this.getFormattedMessage(message, field, args);

        if (!this.errors.hasOwnProperty(field)) {
            this.errors[field] = {};
        }

        this.errors[field][validator] = message;
    }

    validateString(value) {
        return typeof value === 'string';
    }

    validateNumber(value) {
        return !isNaN(value);
    }

    validateBoolean(value) {
        return typeof value === 'boolean';
    }

    validateRequired(value) {
        if (value === undefined || value === null) {
            return false;
        }

        return true;
    }

    validateLength(value, length) {
        return String(value).length === parseInt(length);
    }

    validateFilled(value) {
        return this.validateMin(value, 1);
    }

    validateMin(value, min) {
        return value.length >= min;
    }

    validateMax(value, max) {
        return value.length <= max;
    }
    
    validateLessThanOrEqual(value, max) {
        return parseFloat(value) <= parseFloat(max);
    }
    
    validateGreaterThanOrEqual(value, min) {
        return parseFloat(value) >= parseFloat(min);
    }

    validateEmail(value) {
        return (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(value));
    }

    validateRepeated(value, other) {
        return value == this.data[other];
    }
}