window.MamiaAssistant = (function () {
  function getCsrfToken() {
    const meta = document.querySelector('meta[name="csrf-token"]');
    return meta ? meta.getAttribute('content') || '' : '';
  }

  function initAssistant() {
    const widget = document.querySelector('[data-ai-widget]');
    if (!widget) {
      return;
    }

    const panel = widget.querySelector('.ai-panel');
    const openButton = widget.querySelector('[data-ai-open]');
    const closeButton = widget.querySelector('[data-ai-close]');
    const minimizeButton = widget.querySelector('[data-ai-minimize]');
    const form = widget.querySelector('[data-ai-form]');
    const input = widget.querySelector('[data-ai-input]');
    const messages = widget.querySelector('[data-ai-messages]');
    const typing = widget.querySelector('[data-ai-typing]');
    const suggestionsContainer = widget.querySelector('[data-ai-suggestions]');

    const chatEndpoint = widget.getAttribute('data-chat-endpoint');
    const suggestionsEndpoint = widget.getAttribute('data-suggestions-endpoint');
    const fallbackErrorMessage = widget.getAttribute('data-fallback-error-message') || 'I am unable to answer right now.';

    let conversationId = null;
    let inFlightController = null;

    function autoResizeInput() {
      input.style.height = 'auto';
      input.style.height = Math.min(input.scrollHeight, 132) + 'px';
    }

    function nowTime() {
      const now = new Date();
      return now.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    }

    function appendMessage(role, text, timestamp) {
      const article = document.createElement('article');
      article.className = role === 'user' ? 'ai-message ai-message-user' : 'ai-message ai-message-assistant';

      const bubble = document.createElement('div');
      bubble.className = 'ai-bubble';
      bubble.textContent = text;

      const time = document.createElement('time');
      time.className = 'ai-time';
      time.textContent = timestamp || nowTime();

      article.appendChild(bubble);
      article.appendChild(time);
      messages.appendChild(article);
      requestAnimationFrame(function () {
        messages.scrollTop = messages.scrollHeight;
      });
    }

    function setTyping(isTyping) {
      typing.hidden = !isTyping;
      widget.classList.toggle('is-thinking', isTyping);
      if (isTyping) {
        requestAnimationFrame(function () {
          messages.scrollTop = messages.scrollHeight;
        });
      }
    }

    function toggleOpen(open) {
      widget.classList.toggle('is-open', open);
      widget.classList.toggle('is-closing', !open);
      if (open) {
        widget.classList.remove('is-minimized');
        setTimeout(function () {
          input.focus();
          autoResizeInput();
          messages.scrollTop = messages.scrollHeight;
        }, 140);
      }
    }

    function applySuggestions(suggestions) {
      if (!Array.isArray(suggestions) || suggestions.length === 0) {
        return;
      }

      suggestionsContainer.innerHTML = '';
      suggestions.forEach(function (text) {
        const btn = document.createElement('button');
        btn.type = 'button';
        btn.className = 'ai-suggestion';
        btn.textContent = text;
        btn.setAttribute('data-ai-suggestion', text);
        suggestionsContainer.appendChild(btn);
      });
    }

    async function sendMessage(message) {
      if (inFlightController) {
        inFlightController.abort();
      }

      inFlightController = new AbortController();
      setTyping(true);

      try {
        const response = await fetch(chatEndpoint, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'X-CSRF-TOKEN': getCsrfToken()
          },
          body: JSON.stringify({ message: message, conversationId: conversationId }),
          signal: inFlightController.signal
        });

        if (!response.ok) {
          const errorPayload = await response.json().catch(function () { return null; });
          const errorMessage = errorPayload && errorPayload.message ? errorPayload.message : fallbackErrorMessage;
          throw new Error(errorMessage);
        }

        const payload = await response.json();
        conversationId = payload.conversationId || conversationId;
        appendMessage('assistant', payload.message, formatServerTime(payload.timestamp));
        applySuggestions(payload.suggestions || []);
      } catch (error) {
        if (error.name !== 'AbortError') {
          appendMessage('assistant', error.message || fallbackErrorMessage, nowTime());
        }
      } finally {
        setTyping(false);
        inFlightController = null;
      }
    }

    function formatServerTime(timestamp) {
      if (!timestamp) {
        return nowTime();
      }

      const date = new Date(timestamp);
      if (Number.isNaN(date.getTime())) {
        return nowTime();
      }

      return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    }

    async function loadInitialSuggestions() {
      try {
        const response = await fetch(suggestionsEndpoint, { method: 'GET' });
        if (!response.ok) {
          return;
        }

        const payload = await response.json();
        applySuggestions(payload.suggestions || []);
      } catch {
        // Keep default inline suggestions if API retrieval fails.
      }
    }

    openButton.addEventListener('click', function () { toggleOpen(true); });
    closeButton.addEventListener('click', function () { toggleOpen(false); });
    minimizeButton.addEventListener('click', function () {
      widget.classList.add('is-minimized');
      widget.classList.remove('is-open');
    });

    widget.addEventListener('click', function (event) {
      const target = event.target;
      if (!(target instanceof HTMLElement)) {
        return;
      }

      const suggestion = target.getAttribute('data-ai-suggestion');
      if (!suggestion) {
        return;
      }

      appendMessage('user', suggestion, nowTime());
      input.value = '';
      sendMessage(suggestion);
    });

    input.addEventListener('keydown', function (event) {
      if (event.key === 'Enter' && !event.shiftKey) {
        event.preventDefault();
        form.requestSubmit();
      }

      if (event.key === 'Escape') {
        event.preventDefault();
        toggleOpen(false);
      }
    });

    input.addEventListener('input', autoResizeInput);

    form.addEventListener('submit', function (event) {
      event.preventDefault();
      const value = input.value.trim();
      if (!value) {
        return;
      }

      appendMessage('user', value, nowTime());
      input.value = '';
      autoResizeInput();
      sendMessage(value);
    });

    autoResizeInput();
    loadInitialSuggestions();
  }

  return { initAssistant: initAssistant };
})();
