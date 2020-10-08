<template>
  <div id="app">
    <div id="nav" v-if="currentRouteMeta.showNavBar">
      <router-link to="/">Home</router-link> |
      <router-link to="/about">About</router-link>
    </div>
    <router-view/>
  </div>
</template>

<style lang="scss">
@import 'assets/my.scss';

// Import Bootstrap and BootstrapVue source SCSS files
@import '~bootstrap/scss/bootstrap.scss';
@import '~bootstrap-vue/src/index.scss';
@import "../node_modules/bootstrap/scss/variables";
@import "../node_modules/bootstrap/scss/bootstrap";
@import "../node_modules/bootstrap/scss/functions";
@import "../node_modules/bootstrap/scss/variables";
@import "../node_modules/bootstrap/scss/mixins";

#app {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #714216;
}

#nav {
  padding: 30px;

  a {
    font-weight: bold;
    color:#aecf00;

    &.router-link-exact-active {
      color:#714216;
    }
  }
}
</style>

<script lang="ts">
import Component from 'vue-class-component'
import VueRouter, { RouteConfig, Route } from 'vue-router'
import Vue from 'vue'
import api from './api'

@Component({
})
export default class App extends Vue {
  public currentRoute: Route|null = null;
  public currentRouteMeta: any = null;

  public async created () {
    this.fillRouteInfo(this.$router.currentRoute)
    this.$router.onError(err => this.$bvToast.toast(err.message, { title: 'Navigation error', variant: 'error', solid: true }))
    this.$router.afterEach((to, from) => this.fillRouteInfo(to))
    this.$router.beforeEach(async (to, from, next) => {
      const loggedIn = await api.authorized()
      if (!loggedIn && to.fullPath.startsWith('/app')) {
        next('/')
      } else if (loggedIn && to.name === 'Home') {
        next('/app/topics')
      } else {
        next()
      }
    })

    if (await api.authorized()) {
      if (this.currentRoute != null && this.currentRoute.name === 'Home') {
        this.$router.push('/app/topics')
      }
    } else {
      if (this.$route.fullPath !== '/' && this.$route.fullPath !== '/apply') {
        this.$router.push('/')
      }
    }
  }

  private fillRouteInfo (route: Route) {
    this.currentRoute = route
    this.currentRouteMeta = route.meta
  }
}

</script>
