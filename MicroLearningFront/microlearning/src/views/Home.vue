<template>
  <!--
  <div id="home" class="row">
    <div class="column left">
      <div class="form-container">
        <b-img src="/img/logo.svg" id="logo" alt="logo"></b-img>
      </div>
    </div>
    <div class="column right">
      <div class="form-container">
  -->
  <b-container style="position: relative;" class="bv-example-row bv-example-row-flex-cols d-flex align-items-center min-vh-50">
    <b-row align-v="center" align-h="center" class="container text-center">
      <b-col >
        <b-img src="/img/logo.svg"  alt="logo"></b-img>
        <!-- <img alt="logo" src="../assets/logo.svg"> -->
      </b-col>
      <b-col>
        <SimpleSpinnerComponent :visible="working" />

        <b-form @submit="doSubmit" @reset="doReset" ref="form">
          <b-card class="mt-3" v-bind:header="HomePageModeEnum[mode]">

            <b-form-group >
              <b-form-input v-model="login" required type="text" placeholder="Enter login" ref="txtLogin" ></b-form-input>
            </b-form-group>
            <transition name="slide-fade">
              <b-form-group v-if='mode !== HomePageMode.Login'>
                <b-form-input v-model="email" v-bind:state="isEmailOk" @input="onEmailChange" required type="email"  placeholder="Enter email"></b-form-input>
              </b-form-group>
            </transition>
            <transition name="slide-fade">
              <b-form-group v-if='mode === HomePageMode.Recover'>
                <b-form-input v-model="email2" v-bind:state="isEmailOk" @input="onEmailChange" required type="email"  placeholder="Repeat email"></b-form-input>
              </b-form-group>
            </transition>
            <transition name="slide-fade">
              <b-form-group v-if='mode !== HomePageMode.Recover'>
                <b-form-input v-model="password" v-bind:state="isPasswordOk" @input="onPasswordChange" required type="password"  placeholder="Enter password"></b-form-input>
              </b-form-group>
            </transition>
            <transition name="slide-fade">
              <b-form-group v-if='mode === HomePageMode.Register'>
                <b-form-input v-model="password2" v-bind:state="isPasswordOk" @input="onPasswordChange" required type="password"  placeholder="Repeat password"></b-form-input>
              </b-form-group>
            </transition>

            <b-form-group v-if="mode === HomePageMode.Register">
              <b-button variant="outline-primary" @click="doSwitchRegisterMode">Back</b-button>
              &nbsp;
              <b-button type="submit" variant="success">Sign-up!</b-button>
              &nbsp;
              <b-button type="reset" variant="outline-primary" >Reset</b-button>
            </b-form-group>
            <b-form-group v-if="mode === HomePageMode.Login">
              <b-button variant="outline-primary" @click="doSwitchRegisterMode" >Register</b-button>
              &nbsp;
              <b-button type="submit" variant="success">Sign-in!</b-button>
              &nbsp;
              <b-button type="reset" variant="outline-primary" @click="doSwitchRecoverMode">Recover</b-button>
            </b-form-group>
            <b-form-group v-if="mode === HomePageMode.Recover">
              <b-button type="reset" variant="outline-primary" >Reset</b-button>
              &nbsp;
              <b-button type="submit" variant="success">Recover!</b-button>
              &nbsp;
              <b-button type="reset" variant="outline-primary" @click="doSwitchRecoverMode">Back</b-button>
            </b-form-group>
          </b-card>
        </b-form>

        <transition name="fade">
          <b-alert v-if='mode === HomePageMode.Register && registerSubmitted && (password !== password2)' show variant="warning">Passwords are not matched</b-alert>
        </transition>
        <transition name="fade">
          <b-alert v-if='mode === HomePageMode.Recover && recoverSubmitted && (email !== email2)' show variant="warning">Emails are not matched</b-alert>
        </transition>
        <transition name="fade">
          <b-alert v-if='errorMessage.length > 0' show variant="warning">{{errorMessage}}</b-alert>
        </transition>
  <!--
      </div>
    </div>
  </div>
  -->
      </b-col>
    </b-row>
  </b-container>
</template>

<script lang="ts">
// @ is an alias to /src
import Component from 'vue-class-component'
import SimpleSpinnerComponent from '../components/SimpleSpinnerComponent.vue'
import * as x from '../../xml/my-json-types'
import Vue from 'vue'
import api from '../api'

export enum HomePageMode {
  Login,
  Register,
  Recover
}

@Component({
  components: { SimpleSpinnerComponent }
})
export default class Home extends Vue {
  HomePageMode = { Login: HomePageMode.Login, Register: HomePageMode.Register, Recover: HomePageMode.Recover }
  HomePageModeEnum = HomePageMode
  mode = HomePageMode.Login
  registerSubmitted = false
  recoverSubmitted = false
  isPasswordOk: boolean|null = null
  isEmailOk: boolean|null = null

  login = ''
  email = ''
  email2 = ''
  password = ''
  password2 = ''

  errorMessage = ''
  working = false

  async onPasswordChange () {
    if (this.mode === HomePageMode.Register) {
      if (this.password.length > 0 || this.password2.length > 0) {
        this.isPasswordOk = this.password === this.password2
      } else {
        this.isPasswordOk = null
      }
    } else {
      this.isPasswordOk = null
    }
  }

  async onEmailChange () {
    if (this.mode === HomePageMode.Recover) {
      if (this.email.length > 0 || this.email2.length > 0) {
        this.isEmailOk = this.email === this.email2
      } else {
        this.isEmailOk = null
      }
    } else {
      this.isEmailOk = null
    }
  }

  async doSwitchRegisterMode () {
    if (this.mode === HomePageMode.Login) {
      this.mode = HomePageMode.Register
    } else {
      this.mode = HomePageMode.Login
    }
    this.doReset()
  }

  async doSwitchRecoverMode () {
    if (this.mode === HomePageMode.Login) {
      this.mode = HomePageMode.Recover
    } else {
      this.mode = HomePageMode.Login
    }
    this.doReset()
  }

  async doReset () {
    this.login = ''
    this.email = ''
    this.email2 = ''
    this.password = ''
    this.password2 = ''
    this.registerSubmitted = false
    this.recoverSubmitted = false
    this.errorMessage = ''
    this.isPasswordOk = null
    this.isEmailOk = null;
    (this.$refs.txtLogin as HTMLInputElement).focus()
  }

  async doSubmit (e: Event) {
    e.preventDefault()
    this.working = true
    try {
      switch (this.mode) {
        case HomePageMode.Login:
          await api.login(this.login, this.password)
          this.$router.push('/app/topics')
          break
        case HomePageMode.Register:
          this.registerSubmitted = true
          if (this.password === this.password2) {
            await api.register(this.login, this.password, this.email)
            this.$bvToast.toast('Email having activation link was sent to your email', {
              title: 'Register',
              solid: true,
              noAutoHide: true
            })
            this.doSwitchRegisterMode()
          }
          break
        case HomePageMode.Recover:
          this.recoverSubmitted = true
          if (this.email === this.email2) {
            await api.requestAccessRestore(this.login, this.email)
            this.$bvToast.toast('Email having restore link was sent to your email', {
              title: 'Access restore',
              solid: true,
              noAutoHide: true
            })
            this.doSwitchRecoverMode()
          }
          break
        default:
          throw new Error('Unknown Home page mode ' + this.mode)
      }
      this.errorMessage = ''
    } catch (err) {
      this.errorMessage = err.message
    } finally {
      this.working = false
    }
  }
}
</script>
