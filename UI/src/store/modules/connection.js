const connection = {
    namespaced: true,
    state() {
        return {
            processorAddress: '',
            socket: null,
        };
    },
    mutations: {
        setProcessorAddress(state, processorAddress) {
            state.processorAddress = processorAddress;
        },
        setSocket(state, socket) {
            state.socket = socket;
        },
        closeSocket(state) {
            state.socket?.close();
            state.socket = null;
        },
    },
    actions: {},
    getters: {},
};

export default connection;
