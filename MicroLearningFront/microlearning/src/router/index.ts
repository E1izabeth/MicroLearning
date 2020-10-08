import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import Home from '../views/Home.vue'

Vue.use(VueRouter)

const routes: Array<RouteConfig> = [
  {
    path: '/',
    name: 'Home',
    meta: { showNavBar: true },
    component: Home
  },
  {
    path: '/about',
    name: 'About',
    meta: { showNavBar: true },
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ '../views/About.vue')
  },
  {
    path: '/apply',
    name: 'ApplyKey',
    meta: { showNavBar: false },
    component: () => import(/* webpackChunkName: "about" */ '../views/ApplyKey.vue')
  },
  {
    path: '/app',
    name: 'App',
    meta: { showNavBar: false },
    component: () => import(/* webpackChunkName: "about" */ '../views/Main.vue'),
    children: [
      {
        path: 'topics',
        name: 'Topics',
        meta: { showNavBar: false },
        component: () => import(/* webpackChunkName: "about" */ '../views/Topics.vue'),
        children: [
          {
            path: ':topicId',
            name: 'Topic contents',
            meta: { showNavBar: false },
            component: () => import(/* webpackChunkName: "about" */ '../views/TopicContents.vue')
          },
          {
            path: ':topicId/properties',
            name: 'Topic properties',
            meta: { showNavBar: false },
            component: () => import(/* webpackChunkName: "about" */ '../views/TopicProperties.vue')
          }
        ]
      },
      {
        path: 'resources',
        name: 'Resources',
        meta: { showNavBar: false },
        component: () => import(/* webpackChunkName: "about" */ '../views/Resources.vue')
      },
      {
        path: 'profile',
        name: 'Profile',
        meta: { showNavBar: false },
        component: () => import(/* webpackChunkName: "about" */ '../views/Profile.vue')
      }
    ]
  }
]

const router = new VueRouter({
  routes
})

export default router
