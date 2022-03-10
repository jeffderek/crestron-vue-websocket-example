import * as CrComLib from '@crestron/ch5-crcomlib/build_bundles/amd/cr-com-lib';

function pulse(join) {
    CrComLib.publishEvent('b', join, true); // Press
    CrComLib.publishEvent('b', join, false); // Release
}

export default function processMutation(store, mutation) {
    console.log(mutation);
    switch (mutation.type) {
        case 'counter/incrementCounter': {
            pulse(1);
            break;
        }
        case 'counter/decrementCounter': {
            pulse(2);
            break;
        }
        case 'displays/setPower': {
            switch (mutation.payload.id) {
                case 'display_1': {
                    if (mutation.payload.power) {
                        pulse(11);
                    } else {
                        pulse(12);
                    }
                    break;
                }
                case 'display_2': {
                    if (mutation.payload.power) {
                        pulse(13);
                    } else {
                        pulse(14);
                    }
                    break;
                }
            }
        }
    }
}
