Vue.component('x-window', {
    props: {
        title: {
            type: String,
            required: true,
        },
        width: {
            type: String,
            required: false,
            default: '6',
        },
        slotBody: {
            type: Boolean,
            required: false,
            default: true,
        },
        closeEvent: {
            type: String,
            required: false,
            default: null,
        }
    },
    computed: {
        size() {
            return 'col-' + this.width;
        },
        showClose() {
            return this.closeEvent !== null;
        }
    },
    methods: {
        close() {
            TriggerEvent(this.closeEvent);
        }
    },
    template: `
    <div class="container h-100">
            <div class="row h-100 justify-content-center align-items-center">
                <div :class="{'align-middle': true, [size]: true}">
                    <div class="card">
                        <div class="card-header">
                            <span v-html="title"></span>

                            <button type="button" class="close" @click="close" v-show="showClose">
                                <span>&times;</span>
                            </button>
                        </div>
                        <div v-if="slotBody" class="card-body">
                            <slot></slot>
                        </div>
                        <div v-else>
                            <slot></slot>
                        </div>
                    </div>
                </div>
            </div>
        </div>
`
})