import { createMemoryHistory, createRouter } from 'vue-router';
import Home from '../views/Home.vue';
import Displays from '../views/Displays.vue';

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
];

const router = createRouter({
    history: createMemoryHistory(),
    routes,
});

export default router;
