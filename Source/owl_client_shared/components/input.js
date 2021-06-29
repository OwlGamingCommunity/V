Vue.component('x-input', {
    props: {
        displayName: {
            required: true,
        },
        name: {
            required: true,
        },
        value: {
            type: String,
        },
        values: {
            type: Array,
            required: false,
        },
        errors: {
            required: true,
        },
        type: {
            default: "text",
            type: String
        },
        hideLabel: {
            type: Boolean,
            required: false,
            default: false,
        },
        inTable: {
            type: Boolean,
            required: false,
            default: false,
        },
        displayKey: {
            type: String
        },
        inputKey: {
            tpye: String,
            required: false,
        }
    },
    data: function () {
        return {
            customTypes: ['radio', 'select', 'checkbox', 'money'],
            checked: [],
        }
    },
    methods: {
        updateValue: function (value) {
            if (this.type !== 'checkbox') {
                this.$emit('input', value);
                return;
            }

            // Checkboxes logic
            if(this.checked.includes(value)) {
                this.checked.splice(this.checked.indexOf(value), 1)
            } else {
                this.checked.push(value);
            }
            this.$emit('input', this.checked);
        },
        computeValue(val, key) {
            return ['string', 'number'].includes(typeof val) ? key : val[this.inputKey];
        },
        computeDisplay(val) {
            return ['string', 'number'].includes(typeof val) ? val : val[this.displayKey];
        }
    },
    computed: {
        selectTitle() {
            if (!this.value) {
                return this.displayName;
            }

            if (!this.inputKey) {
                return this.value;
            }

            
            let item = this.values.filter(val => {
                return val[this.inputKey] == this.value;
            }).shift();

            if (!item) {
                return this.displayName;
            }

            return item[this.displayKey];
        }
    },
    template: `
<div class="form-group" :class="{'mb-0': inTable}">
    <label :for="name" v-show="!hideLabel || errors">
        <span v-show="!hideLabel">{{ displayName }}</span>
        <span v-show="errors" class="text-danger animated fadeIn">
            <span class="fa fa-exclamation-triangle"></span>
            <span v-for="error in errors">{{ error }}</span>
        </span>
    </label>

    <div v-if="!customTypes.includes(type)">
        <input
            :type="type"
            :id="name"
            :placeholder="displayName"
            class="form-control"
            :value="value"
            @input="updateValue($event.target.value)"
            required
        >
    </div>
    <div v-else-if="type === 'radio'">
        <span v-for="(label, val) in values">
            <input
                type="radio"
                :name="name"
                :value="val"
                @input="updateValue($event.target.value)"
            > <label class="mr-3">{{ label }}</label>
        </span>
    </div>
    <div v-else-if="type === 'money'">
        <div class="input-group mb-3">
            <div class="input-group-prepend">
                <span class="input-group-text">$</span>
            </div>
            <input
                type="number"
                :id="name"
                :placeholder="displayName"
                class="form-control"
                :value="value"
                @input="updateValue($event.target.value)"
                step="0.01"
                required
            >
        </div>
    </div>
    <div v-else-if="type === 'checkbox'">
        <div v-for="(val, key) in values" class="form-check">
            <input
                type="checkbox"
                class="form-check-input"
                :id="name + computeValue(val, key)"
                :name="name + computeValue(val, key)"
                :value="computeValue(val, key)"
                @input="updateValue($event.target.value)"
            > <label class="mr-3 form-check-label" :for="name + computeValue(val, key)">{{ computeDisplay(val) }}</label>
        </div>
    </div>

    <div v-else-if="type === 'select'">
        <!-- Regular select inputs do not work with CEF, so we're hacking one with a bootstrap dropdown. -->

        <div class="dropdown">
            <button class="btn dropdown-toggle text-left" type="button" data-toggle="dropdown" style="width: 100%;">
                {{ selectTitle }}
            </button>
            <div class="dropdown-menu">
                <a
                    v-for="(val, key) in values"
                    :key="key"
                    class="dropdown-item"
                    href="#"
                    @click.prevent="updateValue(computeValue(val, key))"
                >{{ computeDisplay(val) }}</a>
            </div>
        </div>
    </div>
</div>
`
})