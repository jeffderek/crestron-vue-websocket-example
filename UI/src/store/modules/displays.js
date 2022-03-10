const displays = {
    namespaced: true,
    state() {
        return {
            displayList: {
                display_1: {
                    id: 'display_1',
                    name: 'Left Display',
                    power: false,
                },
                display_2: {
                    id: 'display_2',
                    name: 'Right Display',
                    power: false,
                },
            },
        };
    },
    mutations: {
        setPowerFeedback(state, { id, power }) {
            state.displayList[id].power = power;
        },
        setPower(state, { id, power }) {
            // Empty mutation for Vuex
        },
    },
    actions: {},
    getters: {},
};

export default displays;
