const counter = {
    namespaced: true,
    state() {
        return {
            value: 0,
        };
    },
    mutations: {
        setCounter(state, value) {
            state.value = value;
        },
        incrementCounter(state) {
            // Empty Mutation for Vuex
        },
        decrementCounter(state) {
            // Empty Mutation for Vuex
        },
    },
    actions: {},
    getters: {},
};

export default counter;
