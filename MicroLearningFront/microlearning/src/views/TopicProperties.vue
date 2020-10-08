<template>
  <b-modal id="modal-topic-properties" centered title="Topic properties" no-close-on-backdrop no-close-on-esc :hide-header-close="formWaiting" @hidden="onPropertiesHidden" @ok="handleOk">
    <!--
    <DataSumbitionComponent ref="resWaiter" />
    -->

    <b-form @submit.stop.prevent="handleSubmit">
      <b-form-group  label="Title:" v-bind:description="this.tagsString">
        <b-form-input v-model="this.topic.Title" type="text" required disabled></b-form-input>
      </b-form-group>
      <b-form-group label="Start date and time:">
        <div>
          <b-form-datepicker :state="isSelectedStartDateOk" @input="inputStartTime" v-model="selectedStartDate" class="mb-2" required value-as-date></b-form-datepicker>
          <b-form-timepicker :state="isSelectedStartTimeOk" @input="inputStartTime" v-model="selectedStartTime" required></b-form-timepicker>
        </div>
      </b-form-group>
      <b-form-group label="Interval:">
        <b-form-input :state="isSelectedIntervalOk" v-model="selectedInterval" type="text" required placeholder="Days:Hours:Minutes" :formatter="formatIntervalString"></b-form-input>
      </b-form-group>
    </b-form>

    <template v-slot:modal-footer="{ ok, cancel }">
      <b-button :disabled="formWaiting" variant="danger" @click="deleteTopic()" ref="btnTopicDel" style="margin-right: auto;">Delete</b-button>
      <b-button :disabled="formWaiting || (!isSelectedStartDateOk || !isSelectedIntervalOk)" variant="success" @click="ok()" ref="btnTopicOk" >OK</b-button>
      <b-button :disabled="formWaiting" variant="outline-primary" @click="cancel()" >Cancel</b-button>
    </template>
  </b-modal>
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
export default class TopicProperties extends Vue {
  topic = { } as x.TopicInfoType
  tagsString = ''

  selectedStartDate = new Date()
  selectedStartTime = ''
  selectedInterval = ''

  isSelectedIntervalOk = false
  isSelectedStartDateOk = false
  isSelectedStartTimeOk = false

  selectedIntervalValue = 0
  selectedStartValue = 0

  formWaiting = false

  async mounted () {
    try {
      this.topic = await api.getTopicById(Number.parseInt(this.$router.currentRoute.params.topicId))
      this.tagsString = this.topic.AssociationTags ? this.topic.AssociationTags.map(t => t.Word).join(', ') : '---'

      this.selectedIntervalValue = this.topic.DeliveryIntervalSeconds ?? 0

      const totalMinutes = this.selectedIntervalValue / 60
      const minutes = totalMinutes % 60
      const hours = Math.floor(totalMinutes / 60) % 24
      const days = Math.floor(totalMinutes / 60 / 24)
      this.selectedInterval = this.formatIntervalString(days + ':' + hours + ':' + minutes)
      this.inputStartTime()
    } catch (err) {
      await this.$router.push('/app/topics')
      await this.$nextTick()
      this.$bvToast.toast(err.message, { title: 'Error opening topic', variant: 'warning', solid: true })
    }

    this.$bvModal.show('modal-topic-properties')
  }

  async onPropertiesHidden () {
    this.$router.push('/app/topics')
  }

  async topicPropertiesHandleSubmit () {
    alert('')
  }

  async deleteTopic () {
    try {
      api.deleteTopic(this.topic.Id)
      this.topic = await api.getTopicById(Number.parseInt(this.$router.currentRoute.params.topicId))
      await this.goBackAndReloadTopics()
    } catch (err) {
      this.$bvToast.toast(err.message, { title: 'Error deleting topic', variant: 'warning', solid: true })
    }
  }

  async handleOk (bvModalEvt: Event) {
    bvModalEvt.preventDefault() // Prevent modal from closing
    this.handleSubmit()
  }

  async handleSubmit () {
    // this.loadingData = true
    try {
      await api.activateTopicLearningProgress(this.topic.Id, Math.floor(this.selectedStartValue), Math.floor(this.selectedIntervalValue))
      // item.info.IsActive = true
      await this.goBackAndReloadTopics()
    } catch (err) {
      this.$bvToast.toast(err.message, { title: 'Error activating topic', variant: 'warning', solid: true })
    } finally {
      // this.loadingData = false
    }
  }

  async goBackAndReloadTopics () {
    this.$emit('reloadTopics')
    await this.$nextTick()
    this.$bvModal.hide('modal-topic-properties')
  }

  inputStartTime () {
    const timeParts = new RegExp('^(\\d+)\\:(\\d+)\\:(\\d+)$').exec(this.selectedStartTime) ?? ['0', '0', '0']
    const hours = Number.parseInt(timeParts[1] ?? '0')
    const minutes = Number.parseInt(timeParts[2] ?? '0')
    const seconds = Number.parseInt(timeParts[3] ?? '0')
    const selectedDate = this.selectedStartDate
    const selectedDateTime = new Date(selectedDate.getFullYear(), selectedDate.getMonth(), selectedDate.getDate(), hours, minutes, seconds)
    const dueSeconds = (selectedDateTime.valueOf() - Date.now()) / 1000
    this.selectedStartValue = dueSeconds
    this.isSelectedStartTimeOk = dueSeconds >= (60 * 60)

    const selectedDateNoTime = new Date(selectedDate.getFullYear(), selectedDate.getMonth(), selectedDate.getDate(), 0, 0, 0)
    const now = new Date(Date.now())
    const nowDateNoTime = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0)
    this.isSelectedStartDateOk = selectedDateNoTime.valueOf() >= nowDateNoTime.valueOf()
  }

  formatIntervalString (value: string) {
    const regex = new RegExp('^(\\d+)?\\:(\\d+)(:(\\d+))?$')
    const parts = regex.exec(value)
    if (parts) {
      const days = Number.parseInt(parts[1] ?? '0')
      const hours = Number.parseInt(parts[2] ?? '0')
      const minutes = Number.parseInt(parts[4] ?? '0')
      this.isSelectedIntervalOk = days > 0 || hours > 0 || minutes >= 10
      this.selectedIntervalValue = 60 * (minutes + 60 * (hours + 24 * days))
      return days + ':' + hours + ':' + minutes
    } else {
      this.isSelectedIntervalOk = false
      return value
    }
  }
}
</script>
