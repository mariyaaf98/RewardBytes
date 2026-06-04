<#import "template.ftl" as layout>

<@layout.registrationLayout
displayMessage=!messagesPerField.existsError('username','password')
displayInfo=false;
section>

<#if section = "header">

<div class="login-container">

  <div class="login-wrapper">

    <!-- LEFT SIDE -->
    <div class="login-left">

      <img
        src="${url.resourcesPath}/img/reward-login.jpg"
        class="login-image"
      />

    </div>

    <!-- RIGHT SIDE -->
    <div class="login-right">

      <div class="login-card">

        <div class="avatar">
          👤
        </div>

        <h1>Welcome Back</h1>

        <p class="subtitle">
          Sign in to continue to Bytes Rewards
        </p>

        <form
          id="kc-form-login"
          action="${url.loginAction}"
          method="post"
        >

          <div class="form-group">

            <label for="Email">
              Email
            </label>

            <input
              type="text"
              id="username"
              name="username"
              class="form-control"
              placeholder="Enter Email"
              autofocus
            />

          </div>

          <div class="form-group">

            <label for="password">
              Password
            </label>

            <input
              type="password"
              id="password"
              name="password"
              class="form-control"
              placeholder="Enter password"
            />

          </div>

          <input
            type="submit"
            value="LOGIN"
          />

        </form>

      </div>

    </div>

  </div>

</div>

</#if>

</@layout.registrationLayout>