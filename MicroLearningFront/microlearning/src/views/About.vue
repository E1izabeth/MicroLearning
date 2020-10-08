<template>
  <div class="aboutContent">
    <h1>
        About microlearning
    </h1>

    <div class="blocktext">
      <!-- <b-card v-for="item in items" :key="item.message">
        <b-card-title>{{item.title}}</b-card-title>
        <b-card-text>
          {{ item.message }}
        </b-card-text>
      </b-card> -->
      <p>Microlearning is a way of easy learning without additional efforts. Just take half an hour a day to develop yourself.</p>
      <p>Choose the resources to study! Create your own resources and validate existig. Tag resources for everyone.</p>
      <p>Create interesting topics and subscribe to them. Resources by topic will be automatically detected. Set the time interval for getting new material. All topic's resources have been independently audited.</p>
      <p>Follow the progress of the study. See the learning indicator for the selected topics.</p>
      <p>Don't worry if you feel that you haven't mastered the material well enough when you've finished the topic. You can learne this theme again</p>
    </div>

    <div class="blocktext" style="font-weight: bolder; text-align:center; color:#aecf00">
      Join now and broad your mind!
    </div>

  </div>
</template>

<script lang="ts">
import { Vue } from 'vue-property-decorator'

export default {
  data: function () {
    return {
      // items: [
      //   { message: '', title: '1' },
      //   { message: 'Bar', title: '2' }
      // ]
    }
  }
}

</script>

<style scoped lang="scss">
div.blocktext {
    margin-left: auto;
    padding: 2%;
    margin-right: auto;
    width: 50%;
    text-align: left;
    font-size: larger;
}

h1{
  margin-left: auto;
  margin-right: auto;
}

.aboutContent{
  margin: 3%;
}
</style>
