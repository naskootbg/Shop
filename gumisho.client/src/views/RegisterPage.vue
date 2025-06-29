<script setup>
import { reactive, computed, ref } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { minLength, required, email, sameAs, numeric, maxLength, alphaNum } from '@vuelidate/validators';
import { formDataBG } from '@/const';
import RegisterSuccess from '@/components/RegisterSuccess.vue';
import { RegisterMe } from '@/api/identity';

const isSubmitted = ref(false);

const form = reactive({
  name: '',
  email: '',
  phone: '',
  password: '',
  confirmPassword: ''
})
 
const rules = computed(() => {
  return {
    username: { required , alphaNum}, // Name is required
    phone: {
      required, // Phone is required
      numeric,
      minLength: minLength(10),
      maxLength: maxLength(13)
    },
    email: {
      required, // Email is required
      email // Must be a valid email address
    },
    password: {
      required, // Password is required
      minLength: minLength(8) // Password must have at least 8 characters
    },
    confirmPassword: {
      required, // Password confirmation is required
      sameAs: sameAs(form.password) // Must match the value of the entered password
    }
  }
})

const v$ = useVuelidate(rules, form)

async function handleSubmit() {
  // Validate the form fields
  const result = await v$.value.$validate()
  if (!result) {
    alert('The form has errors')
    return
  }
  await RegisterMe(form);
  isSubmitted.value = true; 
//  alert('Form submitted successfully')
}
</script>

<template>
   
    <RegisterSuccess v-if="isSubmitted" />
    <form v-else @submit.prevent="handleSubmit">
      <div class="field-container">
        <input v-model="form.username" type="text" id="username" :placeholder= "formDataBG.username">
        <span v-if="v$.username.$error">{{ v$.username.$errors[0].$message }}</span>
      </div>
      <div class="field-container">
        <input v-model="form.email" type="text" id="email" :placeholder= "formDataBG.email">
        <span v-if="v$.email.$error">{{ v$.email.$errors[0].$message }}</span>
      </div>
      <div class="field-container">
        <input v-model="form.phone" type="text" id="phone" :placeholder= "formDataBG.phone">
        <span v-if="v$.phone.$error">{{ v$.phone.$errors[0].$message }}</span>
      </div>

      <div class="field-container">
        <input v-model="form.password" type="password" id="password" :placeholder= "formDataBG.password">
        <span v-if="v$.password.$error">{{ v$.password.$errors[0].$message }}</span>
      </div>
      <div class="field-container">
        <input v-model="form.confirmPassword" type="password" :placeholder= "formDataBG.passwordConfirm">
        <span v-if="v$.confirmPassword.$error">{{ v$.confirmPassword.$errors[0].$message }}</span>
      </div>

      <button type="submit">{{ formDataBG.register }}</button>
    </form>
     
  </template>
  
  <style scoped>
  form {
    width: 400px;
    margin: 0 auto;
    padding: 30px;
    border-radius: 20px;
  }
 .field-container {
    display: flex;
    flex-direction: column;
    height: auto;
  }
  label {
    text-align: left;
  }
  input {
    display: block;
    box-sizing: border-box;
    border: none;
    outline: none;
    border-bottom: 1px solid #ddd;
    border-radius: 5px;
    font-size: 1em;
    padding: 1px;
    max-height: 50%;
    width: 100%;
  }
  span {
    color: red;
    font-size: 0.8em;
    text-align: left;
  }
  button {
    background-color: #3498db;
    padding: 10px 20px;
    margin-top: 10px;
    border: none;
    color: white;
  }
  </style>
