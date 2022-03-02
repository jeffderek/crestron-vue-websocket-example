import { createStore } from 'vuex';

export default createStore({
    state: {
        counter: 0,
    },
    mutations: {
        setCounter(state, value) {
            state.counter = value;
        },
        incrementCounter(state) {
            state.counter++;
        },
        decrementCounter(state) {
            state.counter--;
        },
    },
    actions: {},
    modules: {},
});
