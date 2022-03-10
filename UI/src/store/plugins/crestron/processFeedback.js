import * as CrComLib from '@crestron/ch5-crcomlib/build_bundles/amd/cr-com-lib';

// This object contains a list of feedback joins and callback functions that should
// be executed whenever the value changes
// The callback function should only change values within the Vuex store, and then
// The rest of the program can react to the data in the store changing.
let feedbackJoins = {
    digital: [
        {
            join: 11,
            callback: function (store, boolValue) {
                // The callback function is called anytime the digital signal changes,
                // whether it's going to high or to low.  In this particular callback,
                // The value is only checked when it is high, to set the power on feedback
                // to true.
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
                // This is the power off feedback join, again checking to see if it
                // is high before committing the mutation.  If you wanted, you could
                // use a single join for this, and commit on high and low.
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
                // For display_2, this example does exactly that.  A single digital join
                // is used for power state, instead of one join for power on and another join
                // for power off
                if (boolValue) {
                    store.commit('displays/setPowerFeedback', {
                        id: 'display_2',
                        power: true,
                    });
                } else {
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

// This function uses the feedbackJoins array created above
// to subscribe to all of the states, and set up the callbacks
// to be invoked.
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
