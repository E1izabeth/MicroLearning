<template>
  <div>
    <b-card title="Card Title" no-body>
      <b-card-header header-tag="nav" align="center">
        <b-nav card-header tabs>
          <b-nav-item to="/app/topics" exact-active-class="active">Subscriptions</b-nav-item>
          <b-nav-item to="/app/resources" exact-active-class="active">Knowledge Base</b-nav-item>
          <b-nav-item to="/app/profile" exact-active-class="active">Settings</b-nav-item>
          <b-button variant="outline-primary" style="margin-left: auto; margin-bottom:auto" @click="doLogout">Logout</b-button>
        </b-nav>
      </b-card-header>

      <b-card-body class="text-center" style="heigh: 100%">
        <router-view></router-view>
      </b-card-body>
    </b-card>
  </div>
</template>

<script lang="ts">
// @ is an alias to /src
import Component from 'vue-class-component'
import * as x from '../../xml/my-json-types'
import Vue from 'vue'
import api from '../api'

@Component({
})
export default class Main extends Vue {
  async doLogout () {
    await api.logout()
    this.$router.push('/')
  }
}

</script>
