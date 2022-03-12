import { createStore } from 'vuex';
import counter from './modules/counter';
import displays from './modules/displays';
import connection from './modules/connection';
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
        connection: connection,
    },
    plugins: [crestron()],
});
