import {
    createMemoryHistory,
    createRouter,
    createWebHistory,
} from 'vue-router';
import Home from '../views/Home.vue';
import Displays from '../views/Displays.vue';
import Counter from '../views/Counter.vue';

const routes = [
    {
        path: '/',
        name: 'Home',
        component: Home,
    },
    {
        path: '/Displays',
        name: 'Displays',
        component: Displays,
    },
    {
        path: '/Counter',
        name: 'Counter',
        component: Counter,
    },
];

const router = createRouter({
    history: createMemoryHistory(),
    routes,
});

export default router;
