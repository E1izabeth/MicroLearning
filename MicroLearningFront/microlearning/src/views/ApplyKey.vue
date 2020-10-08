<template>
  <div class="aboutContent">
    <h1>
        {{title}}
    </h1>

    <div class="blocktext">
        <b-spinner label="Spinning"></b-spinner>
    </div>

    <div class="blocktext" style="font-weight: bolder; text-align:center; color:#aecf00">
      Please wait
    </div>

  </div>
</template>

<script lang="ts">
import Component from 'vue-class-component'
import VueRouter, { RouteConfig, Route } from 'vue-router'
import ListView, { ListViewItem } from '../controls/ListView.vue'
import Vue from 'vue'
import api from '../api'
import * as x from '../../xml/my-json-types'

@Component({
})
export default class ApplyKey extends Vue {
  working = true
  title = 'Arranging action to perform...'

  async mounted () {
    try {
      const action = this.$router.currentRoute.query.action as string
      const key = this.$router.currentRoute.query.key as string

      this.title = 'Performing requested action...'

      switch (action) {
        case 'activate': await api.performActivateAction(key); break
        case 'restore': await api.performRestoreAccessAction(key); break
        default: throw new Error('Unknown action ' + action)
      }

      this.title = 'Requested action successed'

      await this.$nextTick()
      await this.$router.push('/app/profile')
    } catch (err) {
      this.title = 'Requested action failed'
      await this.$nextTick()
      this.$bvToast.toast(err.message, { title: 'Error opening topic', variant: 'warning', solid: true, noAutoHide: true })
      this.working = false
    } finally {
      this.working = false
    }
  }
}
</script>
