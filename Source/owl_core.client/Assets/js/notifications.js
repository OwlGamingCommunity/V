/**
 * This is the persistent notifications manager. Not to be confused with
 * the regular toast notifications that disappear automatically and
 * are not database driven.
 */

const XNotification = Vue.component('x-notification', {
    template: `
        <div class="panel panel-primary notification" @click="notificationClick">
            <div class="panel-heading">{{ title }}</div>
            <div class="panel-body">
                {{ body }}
            </div>
        </div>
    `,
    props: {
        id: {type: Number, required: true},
        title: {type: String, required: true},
        clickEvent: {type: String, required: false},
        body: {type: String, required: true},
    },
    methods: {
        notificationClick() {
            this.$emit('notification-dismissed', this.id);
            if (typeof this.clickEvent === 'string' && this.clickEvent.length > 0) {
                TriggerEvent(this.clickEvent);
            }
            TriggerEvent("PersistentNotifications_Dismissed", this.id);
        }
    }
})

const XNotificationBell = Vue.component('x-notification-bell', {
    components: {XNotification},
    template: `
        <div v-show="notifications.length > 0">
            <div class="row">
            <div class="notification-bell-container">
                <span class="fa fa-bell text-danger fa-2x animated tada pull-right" @click="showing = !showing">
                    <span class="notification-count">{{ notifications.length }}</span>
                </span>
            </div>
            </div>
            <div class="row" v-show="showing">
                <div class="notifications-container">
                    <x-notification 
                        v-for="notification in sortedNotifications"
                        :id="notification.id"
                        :title="notification.title"
                        :click-event="notification.clickEvent"
                        :body="notification.body"
                        @notification-dismissed="dismissNotification"
                    ></x-notification>
                </div>
            </div>
        </div>
    `,
    data() {
        return {
            showing: false,
            notifications: [],
        }
    },
    methods: {
        dismissNotification(id) {
            this.notifications = this.notifications.filter(notification => notification.id !== id);
        }
    },
    computed: {
        sortedNotifications() {
            return this.notifications.sort((a, b) => b.createdAt - a.createdAt)
        }
    }
})

const App = new Vue({
    el: '#notifications',
    components: {XNotificationBell},
    methods: {
        setNotifications(notifications) {
            this.$refs.bell.notifications = notifications;
        },
        pushNotification(notification) {
            this.$refs.bell.notifications.push(notification);
        }
    }
})

function PushPersistentNotification(id, title, clickEvent, body, createdAt) {
    App.pushNotification({id, title, clickEvent, body, createdAt});
}

function SetPersistentNotifications(notifications) {
    App.setNotifications(notifications);
}