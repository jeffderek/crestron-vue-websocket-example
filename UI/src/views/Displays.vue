<template>
    <div id="displays">
        <div
            class="display-frame"
            v-for="display in $store.state.displays.displayList"
            :key="display.id"
        >
            <h1 class="name">{{ display.name }}</h1>
            <button
                class="btn-default"
                :class="{
                    green: $store.state.displays.displayList[display.id].power,
                }"
                @click="setPower({ id: display.id, power: true })"
            >
                On
            </button>
            <button
                class="btn-default"
                :class="{
                    red: !$store.state.displays.displayList[display.id].power,
                }"
                @click="setPower({ id: display.id, power: false })"
            >
                Off
            </button>
        </div>
    </div>
</template>

<script>
import { mapMutations } from 'vuex';

export default {
    methods: {
        ...mapMutations('displays', ['setPower']),
    },
};
</script>

<style lang="scss">
#displays {
    height: 90%;
    display: flex;
    flex-direction: row;
    gap: 1rem;
    align-items: center;
    justify-content: center;

    .display-frame {
        display: grid;
        grid-template-columns: repeat(2, 1fr);

        .name {
            grid-column: 1/3;
        }
    }
}
</style>
