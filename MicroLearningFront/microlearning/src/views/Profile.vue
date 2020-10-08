<template>
  <b-container class="bv-example-row" id="profile-page">
    <b-row class="justify-content-md-center">
      <b-col>
        <b-card style="margin-left: 10%" header="Your profile settings">
          <SimpleSpinnerComponent :visible="loadingData" />

          <b-form-group label="Login:" description="Your username cannot be changed.">
            <b-form-input id="data-login"  v-model="currentLogin" type="text" disabled></b-form-input>
          </b-form-group>

          <b-form inline @submit="onSubmitNewEmail">
            <b-form-group label="Email:" description="Reenter old one to change it">
              <b-form-input id="data-current-email" v-bind:disabled="this.emailChanged" v-model="mailCurrentEmail" type="email" required v-bind:placeholder="currentEmail" ></b-form-input>
            </b-form-group>
            <div class="break" />
            <b-form-group >
              <b-form-input id="data-mail-new-email" v-bind:disabled="this.emailChanged" v-model="mailNewEmail" v-bind:state="isEmailOk" type="email" @input="onEmailChange" required placeholder="New email"></b-form-input>
            </b-form-group>
            <b-form-group >
              <b-form-input id="data-mail-repeat-email" v-bind:disabled="this.emailChanged" v-model="mailNewEmail2" v-bind:state="isEmailOk" type="email" @input="onEmailChange" required placeholder="Repeat new email"></b-form-input>
            </b-form-group>
            <b-form-group >
              <b-form-input id="data-mail-password" v-bind:disabled="this.emailChanged" v-model="mailCurrentPassword" type="password" required placeholder="Current password"></b-form-input>
            </b-form-group>
            <b-button type="submit" v-bind:disabled="this.emailChanged" variant="success" >Change email!</b-button>
          </b-form>

          <b-form inline @submit="onSubmitNewPassword" >
            <b-form-group  label="Password:" >
              <b-form-input id="data-password-current-password" v-model="currentPassword" type="password" v-bind:placeholder="currentPassword" disabled></b-form-input>
            </b-form-group>
            <div class="break" />
            <b-form-group >
              <b-form-input id="data-password-new-password" v-bind:disabled="this.passwordChanged" v-model="passwordNewPassword" v-bind:state="isPasswordOk" @input="onPasswordChange" type="password" required placeholder="New password"></b-form-input>
            </b-form-group>
            <b-form-group >
              <b-form-input id="data-password-repeat-password" v-bind:disabled="this.passwordChanged" v-model="passwordNewPassword2" v-bind:state="isPasswordOk" @input="onPasswordChange" type="password" required placeholder="Repeat new password"></b-form-input>
            </b-form-group>
            <b-form-group >
              <b-form-input id="data-password-current-email" v-bind:disabled="this.passwordChanged" v-model="passwordCurrentEmail" type="email" required placeholder="Current email"></b-form-input>
            </b-form-group>
            <b-button type="submit" v-bind:disabled="this.passwordChanged" variant="success" >Change password!</b-button>
          </b-form>

          <b-form v-if="!this.isProfileActivated" inline @submit="onRequestActivation" >
            <b-form-group description="Reenter email to ensure">
              <label >Request activation link to the current email:</label>
            </b-form-group>
            <div class="break" />
            <b-form-group>
              <b-form-input id="data-activation-email" v-model="activationCurrentEmail" type="email" required v-bind:placeholder="currentEmail"></b-form-input>
            </b-form-group>
            <b-button type="submit" variant="success" class="align-bottom">Request new activation link!</b-button>
          </b-form>

        </b-card>
      </b-col>
    </b-row>
  </b-container>
</template>

<script lang="ts">
import Component from 'vue-class-component'
import VueRouter, { RouteConfig, Route } from 'vue-router'
import Vue from 'vue'
import api from '../api'
import SimpleSpinnerComponent from '../components/SimpleSpinnerComponent.vue'

@Component({
  components: { SimpleSpinnerComponent }
})
export default class Profile extends Vue {
  currentLogin = ''
  currentEmail = ''
  currentPassword = ''

  mailCurrentEmail = ''
  mailNewEmail = ''
  mailNewEmail2 = ''
  mailCurrentPassword = ''

  passwordNewPassword = ''
  passwordNewPassword2 = ''
  passwordCurrentEmail = ''

  activationCurrentEmail = ''

  isProfileActivated = false
  profileEmail = ''

  isPasswordOk: boolean|null = null
  isEmailOk: boolean|null = null

  emailChanged = false
  passwordChanged = false

  public loadingData = false

  async created () {
    await this.setup()
  }

  async setup () {
    const profile = await api.currentProfile()
    this.isProfileActivated = profile.IsActivated
    this.currentLogin = profile.Login
    this.currentEmail = profile.EmailFootprint
    this.currentPassword = '********'
  }

  async onPasswordChange () {
    if (this.passwordNewPassword.length > 0 || this.passwordNewPassword2.length > 0) {
      this.isPasswordOk = this.passwordNewPassword === this.passwordNewPassword2
    } else {
      this.isPasswordOk = null
    }
  }

  async onEmailChange () {
    if (this.mailNewEmail.length > 0 || this.mailNewEmail2.length > 0) {
      this.isEmailOk = this.mailNewEmail === this.mailNewEmail2
    } else {
      this.isEmailOk = null
    }
  }

  async onSubmitNewEmail (e: Event) {
    e.preventDefault()
    try {
      this.loadingData = true
      if (this.mailCurrentEmail !== this.mailNewEmail && this.mailNewEmail === this.mailNewEmail2 && this.mailNewEmail2 !== '' && this.mailCurrentEmail !== '') {
        await api.setEmail(this.mailCurrentEmail, this.mailNewEmail, this.mailCurrentPassword)
        this.$bvToast.toast('Your email was successfully changed', {
          title: 'Email',
          solid: true,
          noAutoHide: false
        })
        await this.setup()
        this.emailChanged = true
      }
    } catch (err) {
      this.$bvToast.toast(err.message, { title: 'Error changing email', variant: 'warning', solid: true })
    } finally {
      this.loadingData = false
    }
  }

  async onSubmitNewPassword (e: Event) {
    e.preventDefault()
    try {
      this.loadingData = true
      if (this.passwordCurrentEmail !== '' && this.passwordNewPassword === this.passwordNewPassword2) {
        await api.setPassword(this.passwordCurrentEmail, this.passwordNewPassword)
        this.$bvToast.toast('Your password was successfully changed', {
          title: 'Password',
          solid: true,
          noAutoHide: false
        })
        await this.setup()
        this.passwordChanged = true
      }
    } catch (err) {
      this.$bvToast.toast(err.message, { title: 'Error changing password', variant: 'warning', solid: true })
    } finally {
      this.loadingData = false
    }
  }

  async onRequestActivation (e: Event) {
    e.preventDefault()
    if (this.activationCurrentEmail !== '') {
      await api.requestActivation(this.currentEmail)
      this.$bvToast.toast('Email having activation link was sent to your email', {
        title: 'Activation',
        solid: true,
        noAutoHide: true
      })
    }
  }
}
</script>
