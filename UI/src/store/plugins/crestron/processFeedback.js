import * as CrComLib from '@crestron/ch5-crcomlib/build_bundles/amd/cr-com-lib';

let feedbackJoins = {
    digital: [
        {
            join: 11,
            callback: function (store, boolValue) {
                if (boolValue) {
                    store.commit('displays/setPowerFeedback', {
                        id: 'display_1',
                        power: true,
                    });
                }
            },
        },
        {
            join: 12,
            callback: function (store, boolValue) {
                if (boolValue) {
                    store.commit('displays/setPowerFeedback', {
                        id: 'display_1',
                        power: false,
                    });
                }
            },
        },
        {
            join: 13,
            callback: function (store, boolValue) {
                if (boolValue) {
                    store.commit('displays/setPowerFeedback', {
                        id: 'display_2',
                        power: true,
                    });
                }
            },
        },
        {
            join: 14,
            callback: function (store, boolValue) {
                if (boolValue) {
                    store.commit('displays/setPowerFeedback', {
                        id: 'display_2',
                        power: false,
                    });
                }
            },
        },
    ],
    analog: [
        {
            join: 1,
            callback: function (store, analogValue) {
                store.commit('counter/setCounter', analogValue);
            },
        },
    ],
    serial: [
        {
            join: 1,
            callback: function (store, serialValue) {
                store.commit('setSystemName', serialValue);
            },
        },
    ],
};

export default function processFeedback(store) {
    feedbackJoins.digital.forEach((digital) => {
        CrComLib.subscribeState(
            'b',
            digital.join.toString(),
            function (boolValue) {
                digital.callback(store, boolValue);
            }
        );
    });
    feedbackJoins.analog.forEach((analog) => {
        CrComLib.subscribeState(
            'n',
            analog.join.toString(),
            function (analogValue) {
                analog.callback(store, analogValue);
            }
        );
    });
    feedbackJoins.serial.forEach((serial) => {
        CrComLib.subscribeState(
            's',
            serial.join.toString(),
            function (serialValue) {
                serial.callback(store, serialValue);
            }
        );
    });
}
