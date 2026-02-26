(() => {
  const modelURL = "/HerbRecognizer/model.json";
  const metadataURL = "/HerbRecognizer/metadata.json";
  const MARK_PROMPT_THRESHOLD = 0.70;

  const recognitionConfig = window.recognitionConfig || {};
  const isAuthenticated = recognitionConfig.isAuthenticated === true;
  const defaultReturnUrl = encodeURIComponent(window.location.pathname + window.location.search);
  const loginUrl = typeof recognitionConfig.loginUrl === "string" && recognitionConfig.loginUrl.length
    ? recognitionConfig.loginUrl
    : `/Identity/Account/Login?ReturnUrl=${defaultReturnUrl}`;

  const state = {
    model: null,
    labels: [],
    labelMap: null,
    labelMapPromise: null,
    popularMap: null,
    popularMapPromise: null,
    modelPromise: null,
    stream: null,
    video: null,
    isCameraOn: false,
    isMarking: false,
    pendingRecognition: null
  };

  const els = {
    startBtn: document.getElementById("recStart"),
    stopBtn: document.getElementById("recStop"),
    captureBtn: document.getElementById("recCapture"),
    fileInput: document.getElementById("recFile"),
    status: document.getElementById("recStatus"),
    preview: document.getElementById("recPreview"),
    placeholder: document.getElementById("recPlaceholder"),
    imagePreview: document.getElementById("recImagePreview"),
    results: document.getElementById("recResults"),
    loading: document.getElementById("recLoading"),
    canvas: document.getElementById("recCanvas"),
    antiForgeryTokenInput: document.querySelector('#recognitionAntiforgeryForm input[name="__RequestVerificationToken"]'),
    markModal: document.getElementById("recMarkModal"),
    markModalText: document.getElementById("recMarkModalText"),
    markModalHint: document.getElementById("recMarkModalHint"),
    markCancelBtn: document.getElementById("recMarkCancelBtn"),
    markLoginBtn: document.getElementById("recMarkLoginBtn"),
    markConfirmBtn: document.getElementById("recMarkConfirmBtn")
  };

  if (!els.startBtn || !els.captureBtn || !els.fileInput || !els.results || !els.canvas) {
    return;
  }

  const normalizeText = (value) => String(value || "").trim().toLocaleLowerCase("bg-BG");

  const escapeHtml = (value) => String(value ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#39;");

  const setStatus = (text) => {
    if (els.status) {
      els.status.textContent = text;
    }
  };

  const setLoading = (isLoading) => {
    if (!els.loading) {
      return;
    }

    els.loading.hidden = !isLoading;
  };

  const showPlaceholder = (show) => {
    if (els.placeholder) {
      els.placeholder.hidden = !show;
    }
  };

  const showImagePreview = (show, src) => {
    if (!els.imagePreview) {
      return;
    }

    if (src) {
      els.imagePreview.src = src;
    }
    els.imagePreview.hidden = !show;
  };

  const removeVideo = () => {
    if (state.video && state.video.parentNode) {
      state.video.parentNode.removeChild(state.video);
    }
    state.video = null;
  };

  const closeMarkModal = () => {
    if (!els.markModal) {
      state.pendingRecognition = null;
      return;
    }

    els.markModal.hidden = true;
    state.pendingRecognition = null;
    if (els.markConfirmBtn) {
      els.markConfirmBtn.disabled = false;
      els.markConfirmBtn.textContent = "Маркирай на картата";
    }
  };

  const openMarkModal = (recognition) => {
    if (!els.markModal || !els.markModalText || !recognition) {
      return;
    }

    const probabilityPercent = Math.round((recognition.probability || 0) * 100);
    els.markModalText.textContent = `Разпозната е „${recognition.className}“ с ${probabilityPercent}% вероятност. Искаш ли да я отбележиш на картата?`;

    if (els.markLoginBtn) {
      els.markLoginBtn.href = loginUrl;
      els.markLoginBtn.hidden = isAuthenticated;
    }

    if (els.markConfirmBtn) {
      els.markConfirmBtn.hidden = !isAuthenticated;
    }

    if (els.markModalHint) {
      if (isAuthenticated) {
        els.markModalHint.hidden = false;
        els.markModalHint.textContent = "Ще използваме текущата ти локация.";
      } else {
        els.markModalHint.hidden = false;
        els.markModalHint.textContent = "За отбелязване на находка е нужен вход в профил.";
      }
    }

    state.pendingRecognition = recognition;
    els.markModal.hidden = false;
  };

  const ensureModel = async () => {
    if (state.model) {
      return state.model;
    }
    if (state.modelPromise) {
      return state.modelPromise;
    }
    if (!window.tf) {
      setStatus("Липсва библиотеката TensorFlow.js.");
      throw new Error("tf не е наличен");
    }

    setStatus("Зареждане на модела...");
    state.modelPromise = (async () => {
      try {
        const [metaRes, modelRes] = await Promise.all([
          fetch(metadataURL, { cache: "no-store" }),
          fetch(modelURL, { cache: "no-store" })
        ]);

        if (!metaRes.ok || !modelRes.ok) {
          throw new Error(`Липсват файлове на модела. metadata: ${metaRes.status}, model: ${modelRes.status}`);
        }

        const metadata = await metaRes.json();
        state.labels = Array.isArray(metadata.labels) ? metadata.labels : [];

        state.model = await tf.loadLayersModel(modelURL);
        setStatus("Готово за разпознаване.");
        return state.model;
      } catch (err) {
        console.error(err);
        setStatus("Грешка при зареждане на модела. Провери файловете в /HerbRecognizer/.");
        state.modelPromise = null;
        throw err;
      }
    })();

    return state.modelPromise;
  };

  const ensureLabelMap = async () => {
    if (state.labelMap) {
      return state.labelMap;
    }
    if (state.labelMapPromise) {
      return state.labelMapPromise;
    }

    state.labelMapPromise = (async () => {
      try {
        const res = await fetch("/Herbs/LabelMap", { cache: "no-store" });
        if (!res.ok) {
          throw new Error(`Неуспешно зареждане на LabelMap: ${res.status}`);
        }

        const data = await res.json();
        const map = new Map();
        if (Array.isArray(data)) {
          data.forEach((item) => {
            if (item && item.latin && item.name) {
              map.set(normalizeText(item.latin), String(item.name));
            }
          });
        }

        state.labelMap = map;
        return map;
      } catch (err) {
        console.error(err);
        state.labelMapPromise = null;
        return null;
      }
    })();

    return state.labelMapPromise;
  };

  const ensurePopularMap = async () => {
    if (state.popularMap) {
      return state.popularMap;
    }
    if (state.popularMapPromise) {
      return state.popularMapPromise;
    }

    state.popularMapPromise = (async () => {
      try {
        const res = await fetch("/Herbs/PopularMap", { cache: "no-store" });
        if (!res.ok) {
          throw new Error(`Неуспешно зареждане на PopularMap: ${res.status}`);
        }

        const data = await res.json();
        const map = new Map();
        if (Array.isArray(data)) {
          data.forEach((item) => {
            if (!item || !item.name || !item.id) {
              return;
            }

            const normalized = normalizeText(item.name);
            if (normalized && !map.has(normalized)) {
              map.set(normalized, {
                id: item.id,
                name: String(item.name)
              });
            }
          });
        }

        state.popularMap = map;
        return map;
      } catch (err) {
        console.error(err);
        state.popularMapPromise = null;
        return null;
      }
    })();

    return state.popularMapPromise;
  };

  const renderResults = (predictions) => {
    setLoading(false);
    if (!predictions || predictions.length === 0) {
      els.results.innerHTML = '<p class="rec-muted">Няма резултати.</p>';
      return;
    }

    const top = predictions[0];
    const pct = Math.round(top.probability * 100);
    els.results.innerHTML = `
      <div class="rec-top">Най-вероятно: <strong>${escapeHtml(top.className)}</strong> (${pct}%)</div>
    `;
  };

  const maybePromptForMarking = (topPrediction) => {
    if (!topPrediction || !topPrediction.herbId) {
      return;
    }

    if (topPrediction.probability < MARK_PROMPT_THRESHOLD) {
      return;
    }

    openMarkModal(topPrediction);
  };

  const markRecognitionAtCurrentLocation = async () => {
    if (!isAuthenticated) {
      window.location.href = loginUrl;
      return;
    }

    if (state.isMarking || !state.pendingRecognition || !state.pendingRecognition.herbId) {
      return;
    }

    if (!navigator.geolocation) {
      setStatus("Геолокацията не се поддържа.");
      return;
    }

    const antiForgeryToken = els.antiForgeryTokenInput?.value ?? "";
    if (!antiForgeryToken) {
      setStatus("Липсва защитен токен. Презареди страницата.");
      return;
    }

    state.isMarking = true;
    if (els.markConfirmBtn) {
      els.markConfirmBtn.disabled = true;
      els.markConfirmBtn.textContent = "Маркиране...";
    }
    setStatus("Определяне на текущата локация...");

    navigator.geolocation.getCurrentPosition(
      async (position) => {
        try {
          const response = await fetch("/HerbFindings/Create", {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
              "RequestVerificationToken": antiForgeryToken
            },
            body: JSON.stringify({
              herbId: state.pendingRecognition.herbId,
              latitude: position.coords.latitude,
              longitude: position.coords.longitude
            })
          });

          if (response.redirected && response.url.includes("/Identity/Account/Login")) {
            window.location.href = response.url;
            return;
          }

          if (response.status === 401 || response.status === 403) {
            window.location.href = loginUrl;
            return;
          }

          if (!response.ok) {
            throw new Error("Неуспешно записване на находката.");
          }

          const markedHerbName = state.pendingRecognition.className;
          closeMarkModal();
          setStatus(`Билката „${markedHerbName}“ беше отбелязана на картата.`);
        } catch (err) {
          console.error(err);
          setStatus("Грешка при отбелязване на находката.");
        } finally {
          state.isMarking = false;
          if (els.markConfirmBtn) {
            els.markConfirmBtn.disabled = false;
            els.markConfirmBtn.textContent = "Маркирай на картата";
          }
        }
      },
      (error) => {
        if (error && error.code === 1) {
          setStatus("Разреши достъп до местоположение, за да отбележиш находка.");
        } else if (error && error.code === 3) {
          setStatus("Изчакването за местоположение изтече. Опитай отново.");
        } else {
          setStatus("Не може да се вземе местоположение.");
        }

        state.isMarking = false;
        if (els.markConfirmBtn) {
          els.markConfirmBtn.disabled = false;
          els.markConfirmBtn.textContent = "Маркирай на картата";
        }
      },
      { enableHighAccuracy: true, timeout: 15000 }
    );
  };

  const predictFromCanvas = async () => {
    closeMarkModal();
    setLoading(true);
    const model = await ensureModel();
    const labelMap = await ensureLabelMap();
    const popularMap = await ensurePopularMap();

    const predictions = await tf.tidy(async () => {
      const image = tf.browser.fromPixels(els.canvas);
      const normalized = image.toFloat().div(127.5).sub(1);
      const batched = normalized.expandDims(0);
      const logits = model.predict(batched);
      const data = await logits.data();
      return Array.from(data);
    });

    const labels = state.labels.length ? state.labels : predictions.map((_, i) => `Клас ${i + 1}`);
    const result = labels.map((label, i) => {
      let name = label;
      if (labelMap && label) {
        const mapped = labelMap.get(normalizeText(label));
        if (mapped) {
          name = mapped;
        }
      }

      const popularMatch = popularMap ? popularMap.get(normalizeText(name)) : null;
      return {
        className: name,
        probability: predictions[i] ?? 0,
        herbId: popularMatch ? popularMatch.id : null
      };
    });

    result.sort((a, b) => b.probability - a.probability);
    renderResults(result);
    maybePromptForMarking(result[0]);
  };

  const getSourceSize = (img) => {
    if (img.videoWidth && img.videoHeight) {
      return { width: img.videoWidth, height: img.videoHeight };
    }
    if (img.naturalWidth && img.naturalHeight) {
      return { width: img.naturalWidth, height: img.naturalHeight };
    }
    return { width: img.width || 0, height: img.height || 0 };
  };

  const drawImageToCanvas = (img) => {
    const ctx = els.canvas.getContext("2d");
    const size = 224;
    els.canvas.width = size;
    els.canvas.height = size;
    ctx.clearRect(0, 0, size, size);

    const { width, height } = getSourceSize(img);
    if (!width || !height) {
      return;
    }

    const side = Math.min(width, height);
    const sx = (width - side) / 2;
    const sy = (height - side) / 2;

    ctx.drawImage(img, sx, sy, side, side, 0, 0, size, size);
  };

  const stopCamera = async () => {
    if (state.stream) {
      state.stream.getTracks().forEach((track) => track.stop());
    }
    state.stream = null;
    removeVideo();
    state.isCameraOn = false;
    showImagePreview(false);
    showPlaceholder(true);
    setStatus("Камерата е спряна.");
  };

  const startCamera = async () => {
    if (state.isCameraOn) {
      return;
    }
    if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
      setStatus("Браузърът не поддържа камера.");
      return;
    }

    await ensureModel();
    setStatus("Стартиране на камера...");

    removeVideo();
    showImagePreview(false);
    showPlaceholder(false);

    const stream = await navigator.mediaDevices.getUserMedia({
      video: { facingMode: { ideal: "environment" } },
      audio: false
    });

    const video = document.createElement("video");
    video.autoplay = true;
    video.playsInline = true;
    video.muted = true;
    video.srcObject = stream;

    await new Promise((resolve) => {
      video.onloadedmetadata = () => resolve();
    });

    state.stream = stream;
    state.video = video;
    state.isCameraOn = true;
    els.preview.appendChild(video);
    setStatus("Камерата работи. Натисни \"Снимай\" за разпознаване.");
  };

  const captureFromCamera = async () => {
    if (!state.video) {
      return;
    }

    drawImageToCanvas(state.video);
    await predictFromCanvas();
  };

  const handleFile = async (file) => {
    if (!file) {
      return;
    }
    if (state.isCameraOn) {
      await stopCamera();
    }

    setStatus("Зареждане на снимката...");
    await ensureModel();
    const url = URL.createObjectURL(file);
    const img = els.imagePreview || new Image();

    img.onload = async () => {
      setStatus("Разпознаване...");
      drawImageToCanvas(img);
      await predictFromCanvas();
      URL.revokeObjectURL(url);
    };

    img.onerror = () => {
      setStatus("Неуспешно зареждане на снимката.");
    };

    showPlaceholder(false);
    if (els.imagePreview) {
      showImagePreview(true, url);
    } else {
      img.src = url;
    }
  };

  els.markCancelBtn?.addEventListener("click", () => {
    closeMarkModal();
  });

  els.markConfirmBtn?.addEventListener("click", async () => {
    await markRecognitionAtCurrentLocation();
  });

  els.markModal?.addEventListener("click", (event) => {
    const closeTarget = event.target.closest("[data-close-modal='true']");
    if (closeTarget) {
      closeMarkModal();
    }
  });

  document.addEventListener("keydown", (event) => {
    if (event.key === "Escape" && els.markModal && !els.markModal.hidden) {
      closeMarkModal();
    }
  });

  els.startBtn.addEventListener("click", async (event) => {
    event.preventDefault();
    try {
      await startCamera();
    } catch (err) {
      setStatus("Грешка при стартиране на камерата.");
      console.error(err);
    }
  });

  els.stopBtn?.addEventListener("click", async (event) => {
    event.preventDefault();
    await stopCamera();
  });

  els.captureBtn.addEventListener("click", async (event) => {
    event.preventDefault();
    if (!state.isCameraOn) {
      setStatus("Първо стартирай камерата.");
      return;
    }

    await captureFromCamera();
  });

  els.fileInput.addEventListener("change", async (event) => {
    const file = event.target.files && event.target.files[0];
    try {
      await handleFile(file);
    } catch (err) {
      console.error(err);
    }
  });
})();
