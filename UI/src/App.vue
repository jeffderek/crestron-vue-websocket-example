<template>
    <NavSidebar />
    <div id="router-container">
        <router-view />
    </div>
</template>

<script>
import NavSidebar from '@/components/NavSidebar';

export default {
    components: {
        NavSidebar,
    },
};
</script>

<style lang="scss">
@import './styles/style.scss';

#app {
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    text-align: center;
    color: #2c3e50;

    height: 100%;
    width: 100%;

    display: flex;
    flex-direction: row;

    #router-container {
        flex: 1;
    }
}
</style>
