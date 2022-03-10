import { createStore } from 'vuex';
import counter from './modules/counter';
import displays from './modules/displays';
import crestron from './plugins/crestron';

export default createStore({
    state() {
        return {
            systemName: '',
        };
    },
    mutations: {
        setSystemName(state, systemName) {
            state.systemName = systemName;
        },
    },
    modules: {
        counter: counter,
        displays: displays,
    },
    plugins: [crestron()],
});
