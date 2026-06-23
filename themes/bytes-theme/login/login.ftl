<#import "template.ftl" as layout>

<@layout.registrationLayout
  displayMessage=false
  displayInfo=false;
  section>

<#if section = "header">

<div class="login-container">

  <div class="login-wrapper">

    <!-- LEFT SIDE -->
    <div class="login-left">
      <img src="${url.resourcesPath}/img/reward-login.jpg" class="login-image" />
    </div>

    <!-- RIGHT SIDE -->
    <div class="login-right">

      <div class="login-card">

        <div class="avatar">👤</div>

        <h1>Welcome Back</h1>
        <p class="subtitle">Sign in to continue to Bytes Rewards</p>

        <#--
          ── GLOBAL ERROR BANNER ──────────────────────────────────────────
          Shown when Keycloak sets a message (wrong credentials, account
          locked, disabled, etc.).  We map the message key to a friendly
          human-readable string.
        -->
        <#if message?has_content && message.type = "error">
          <div class="alert-error">
            <span class="alert-icon">⚠</span>
            <#if message.summary?contains("Invalid user credentials") || message.summary?contains("invalidUserMessage")>
              Incorrect email or password. Please try again.
            <#elseif message.summary?contains("accountDisabled") || message.summary?contains("Account is disabled")>
              Your account has been disabled. Contact your administrator.
            <#elseif message.summary?contains("userTemporarilyDisabled") || message.summary?contains("temporarily disabled")>
              Too many failed attempts. Your account is temporarily locked. Try again later.
            <#elseif message.summary?contains("accountNotVerified")>
              Your email has not been verified. Please check your inbox.
            <#else>
              ${message.summary}
            </#if>
          </div>
        </#if>

        <#if message?has_content && message.type = "warning">
          <div class="alert-warning">
            <span class="alert-icon">ℹ</span>
            ${message.summary}
          </div>
        </#if>

        <form id="kc-form-login" action="${url.loginAction}" method="post">

          <!-- EMAIL -->
          <div class="form-group">
            <label for="username">Email</label>

            <input
              type="text"
              id="username"
              name="username"
              class="form-control <#if messagesPerField.existsError('username')>input-error</#if>"
              placeholder="Enter Email"
              value="${(login.username!'')}"
              autofocus
            />

            <#if messagesPerField.existsError('username')>
              <span class="field-error">
                <#if messagesPerField.get('username')?contains("requiredField") || messagesPerField.get('username')?contains("Please specify")>
                  Email is required.
                <#else>
                  ${messagesPerField.get('username')}
                </#if>
              </span>
            </#if>
          </div>

          <!-- PASSWORD -->
          <div class="form-group">
            <label for="password">Password</label>

            <div class="password-wrapper">
              <input
                type="password"
                id="password"
                name="password"
                class="form-control <#if messagesPerField.existsError('password')>input-error</#if>"
                placeholder="Enter password"
              />

              <button
                type="button"
                class="pw-toggle"
                onclick="togglePassword()"
                aria-label="Show or hide password">
                <svg id="eye-open" xmlns="http://www.w3.org/2000/svg" width="20" height="20"
                  fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.8">
                  <path stroke-linecap="round" stroke-linejoin="round"
                    d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                  <path stroke-linecap="round" stroke-linejoin="round"
                    d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943
                       9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"/>
                </svg>
                <svg id="eye-off" xmlns="http://www.w3.org/2000/svg" width="20" height="20"
                  fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.8"
                  style="display:none">
                  <path stroke-linecap="round" stroke-linejoin="round"
                    d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7
                       a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243
                       M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532
                       l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5
                       c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0
                       01-4.132 5.411m0 0L21 21"/>
                </svg>
              </button>
            </div>

            <#if messagesPerField.existsError('password')>
              <span class="field-error">
                <#if messagesPerField.get('password')?contains("requiredField") || messagesPerField.get('password')?contains("Please specify")>
                  Password is required.
                <#else>
                  ${messagesPerField.get('password')}
                </#if>
              </span>
            </#if>
          </div>

          <input type="submit" value="LOGIN" />

        </form>

      </div>

    </div>

  </div>

</div>

<script>
  function togglePassword() {
    var input   = document.getElementById('password');
    var eyeOpen = document.getElementById('eye-open');
    var eyeOff  = document.getElementById('eye-off');
    if (input.type === 'password') {
      input.type = 'text';
      eyeOpen.style.display = 'none';
      eyeOff.style.display  = 'block';
    } else {
      input.type = 'password';
      eyeOpen.style.display = 'block';
      eyeOff.style.display  = 'none';
    }
  }
</script>

</#if>

</@layout.registrationLayout>
