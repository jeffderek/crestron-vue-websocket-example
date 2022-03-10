<template>
    <div id="nav">
        <img alt="logo" src="../assets/logo.png" />
        <router-link to="/" class="btn-nav">Home</router-link>
        <router-link to="/displays" class="btn-nav">Displays</router-link>
    </div>
</template>

<style lang="scss">
#nav {
    flex-basis: 200px;
    height: 100%;
    background: $primary-color;

    display: flex;
    flex-direction: column;

    img {
        padding: 25px 15px;
        width: 100%;
    }

    .btn-nav {
        margin-left: 1rem;
        padding: 1rem;
        border-radius: 5px 0 0 5px;

        font-weight: bold;
        font-size: 1.2rem;
        text-align: right;
        color: #fff;

        &.router-link-exact-active {
            background-color: #fff;
            color: $primary-color;
        }
    }
}
</style>
