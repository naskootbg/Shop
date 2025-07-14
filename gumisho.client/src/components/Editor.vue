<template>

  <div class="editor-container">
    <!-- Toolbar Header horizontal -->
    <div class="toolbar">


      <button @click="exec('bold')" title="Bold" class="toolbar-btn"><b>B</b></button>
      <button @click="exec('italic')" title="Italic" class="toolbar-btn"><i>I</i></button>
      <button @click="exec('underline')" title="Underline" class="toolbar-btn"><u>U</u></button>

      <button @click="exec('insertUnorderedList')" title="Unordered List" class="toolbar-btn">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
          <circle cx="5" cy="6" r="1.5" />
          <rect x="9" y="5" width="12" height="2" />
          <circle cx="5" cy="12" r="1.5" />
          <rect x="9" y="11" width="12" height="2" />
          <circle cx="5" cy="18" r="1.5" />
          <rect x="9" y="17" width="12" height="2" />
        </svg>
      </button>

      <button @click="exec('insertOrderedList')" title="Ordered List" class="toolbar-btn">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
          <text x="3" y="7" font-size="6">1.</text>
          <rect x="9" y="5" width="12" height="2" />
          <text x="3" y="13" font-size="6">2.</text>
          <rect x="9" y="11" width="12" height="2" />
          <text x="3" y="19" font-size="6">3.</text>
          <rect x="9" y="17" width="12" height="2" />
        </svg>
      </button>
      <select @change="setFontSize($event)" class="toolbar-select2">
        <option disabled selected>H1</option>
        <option v-for="size in fontSizes" :key="size.value" :value="size.value">
          {{ size.label }}
        </option>
      </select>
      <label title="Text Color" class="toolbar-btn" style="display: flex; align-items: center; gap: 4px;">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
          <path d="M5 19h14v2H5zM11 4l-6 14h2l1.5-4h7l1.5 4h2L13 4h-2zm-1.4 8l2.4-6 2.4 6h-4.8z" />
        </svg>
        <input type="color" @input="setColor($event)" class="toolbar-color" />
      </label>
      <label title="Background Color" class="toolbar-btn" style="display: flex; align-items: center; gap: 4px;">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
          <rect x="3" y="3" width="18" height="18" rx="2" ry="2" />
          <rect x="7" y="7" width="10" height="10" fill="white" />
        </svg>
        <input type="color" @input="setBackground($event)" class="toolbar-color" />
      </label>
      <button @click="triggerImageUpload" title="Insert Image" class="toolbar-btn">🖼️</button>
      <input type="file"
             ref="imageInput"
             accept="image/*"
             @change="handleImageUpload"
             style="display: none" />
      <button @click="toggleAlignment('left')" title="Align Left" class="toolbar-btn">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
          <rect x="3" y="5" width="18" height="2" />
          <rect x="3" y="9" width="12" height="2" />
          <rect x="3" y="13" width="18" height="2" />
          <rect x="3" y="17" width="12" height="2" />
        </svg>
      </button>

      <button @click="toggleAlignment('center')" title="Align Center" class="toolbar-btn">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
          <rect x="6" y="5" width="12" height="2" />
          <rect x="4" y="9" width="16" height="2" />
          <rect x="6" y="13" width="12" height="2" />
          <rect x="4" y="17" width="16" height="2" />
        </svg>
      </button>

      <button @click="toggleAlignment('right')" title="Align Right" class="toolbar-btn">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
          <rect x="3" y="5" width="18" height="2" />
          <rect x="9" y="9" width="12" height="2" />
          <rect x="3" y="13" width="18" height="2" />
          <rect x="9" y="17" width="12" height="2" />
        </svg>
      </button>


      <button @click="exec('undo')" title="Undo" class="toolbar-btn">↩️</button>
      <button @click="exec('redo')" title="Redo" class="toolbar-btn">↪️</button>
      <button @click="clear" title="Clear Content" class="toolbar-btn">🧹</button>

      <button @click="downloadHtml" title="Export HTML" class="toolbar-btn">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
          <path d="M5 20h14v-2H5v2zm7-18l-6 6h4v6h4v-6h4l-6-6z" />
        </svg>
      </button>

      <label class="file-upload toolbar-btn" title="Upload .html file">
        📂
        <input type="file" accept=".html" @change="handleHtmlUpload" hidden />
      </label>

      <select @change="insertTemplate($event)" class="toolbar-select">
        <option disabled selected>Insert Template</option>
        <option v-for="key in templateKeys" :key="key" :value="key">{{ key }}</option>
      </select>

    </div>

    <div class="editable"
         contenteditable="true"
         @input="onInput"
         @drop.prevent="handleDrop"
         @dragover.prevent="dragOver = true"
         @dragleave="dragOver = false"
         :class="{ 'drag-over': dragOver }"
         ref="editor">
    </div>


    <h3>
      Raw HTML Output:
      <button @click="loadHtml" title="Load HTML" class="toolbar-btn"> Зареди
        <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
          <path d="M10 17l-5-5 5-5v3h9v4h-9v3z" />
        </svg>
      </button>
    </h3>
    <textarea v-model="html" class="raw-html" />
    <div class="editor-footer">
      <fieldset role="group" class="updated">
        <select v-model="status">
          <option value="0">Получена</option>
          <option value="1">Обработена от шофьор</option>
          <option value="4">Изпрана</option>
          <option value="2">Завършена</option>
          <option value="3">Отказана</option>
          <option value="5">Регистрация</option>
        </select>
        <input v-model="name" type="text" name="name" placeholder="Име на темата" />
        <input v-model="themeid" type="text" name="themeid" style="display:none;" />

        <button @click="Save">Запази</button> <br />
      </fieldset>
    </div>
    <hr />
<table>
  <thead>
    <tr><th>Име</th><th>Статус</th><th></th><th></th></tr>
  </thead>
  <tbody>
    <tr v-for="theme in adminStore.themes" :key="theme.id">
      <td>{{theme.name}}</td>
      <td>{{theme.orderStatus}}</td>
      <td><button @click="Edit(theme.id)">✎</button></td>
      <td><button @click="Del(theme.id)">✖</button></td>
    </tr>
  </tbody>
</table>


  </div>
  
</template>
 

<script setup>
  import { ref, onMounted } from 'vue';
  import { useAdminStore } from '@/stores/useAdminStore';
  import { justMail, ShowTheme, AllThemes, EditTheme, AddTheme, DelTheme } from '@/api/mail.js';

  const html = ref('');
  const editor = ref(null);
  const imageInput = ref(null);
  const dragOver = ref(false);
  const draggedImage = ref(null);

  const name = ref('');
  const status = ref(0);
  const allThemes = ref(null);
  const themeid = ref(0);

  const adminStore = useAdminStore();
  onMounted(async () => {
    var themes = await AllThemes();
    adminStore.themes = themes;
  });
  async function Save() {
    if (themeid.value > 0) {
      var addedTheme = await EditTheme(themeid.value, name.value, name.value, html.value, status.value)
      adminStore.updateTheme(themeid.value, name.value, html.value, status.value);
    }
    else {
      var addedTheme = await AddTheme(name.value, name.value, html.value, status.value)
      adminStore.themes.push(addedTheme);
    }
    
  }
  function Edit(id) {
    var current = adminStore.themes.find(t => t.id == id);
    themeid.value = current.id;
    name.value = current.name;
    html.value = current.htmlContent;
    status.value = current.orderStatus;
  }
  async function Del(id) {
    await DelTheme(id);
    adminStore.themes = adminStore.themes.filter(t => t.id != id);
  }
  const fontSizes = [
    { label: 'Small', value: 2 },
    { label: 'Normal', value: 3 },
    { label: 'Large', value: 5 },
    { label: 'Huge', value: 7 }
  ];

  const templateKeys = ['Header', 'Footer', 'Banner', 'Button', 'Row2', 'Row3', 'Row4'];

  function setColor(e) {
    exec('foreColor', e.target.value);
  }
  function getAlignStyle(direction = 'left') {
    return {
      left: 'text-align:left;margin-left:0;margin-right:auto;',
      center: 'text-align:center;margin-left:auto;margin-right:auto;',
      right: 'text-align:right;margin-left:auto;margin-right:0;'
    }[direction];
  }
 
  function cleanInnerAlignment(container) {
    const nodes = container.querySelectorAll('*');
    nodes.forEach(node => {
      node.style.textAlign = '';
      node.style.marginLeft = '';
      node.style.marginRight = '';
      if (node.getAttribute('style')?.trim() === '') {
        node.removeAttribute('style');
      }
    });
  }
  const year = new Date().getFullYear();
  function getTemplate(key) {
    const style = getAlignStyle('center');
    const map = {
      Header: `<div style="${style}"><h1>Welcome to Our Newsletter</h1></div>`,
      Footer: `<div style="font-size:12px;color:gray;${style}">© ${year} Wash-bg</div>`,
      Banner: `<div style="background:#eee;padding:20px;${style}"><strong>🔥 Special Offer 🔥</strong></div>`,
      Button: `<a href="#" style="background:#007bff;color:white;padding:10px 20px;border-radius:4px;text-decoration:none;display:inline-block;${style}">Click Me</a>`,

      Row2: `<div style="display: flex; justify-content: space-between; gap: 10px;">
      <div style="width: 48%; padding: 10px; background: #eee; text-align: center;">Element 1</div>
      <div style="width: 48%; padding: 10px; background: #eee; text-align: center;">Element 2</div>
    </div>`,

      Row3: `<div style="display: flex; justify-content: space-between; gap: 10px;">
      <div style="width: 32%; padding: 10px; background: #eee; text-align: center;">Element 1</div>
      <div style="width: 32%; padding: 10px; background: #eee; text-align: center;">Element 2</div>
      <div style="width: 32%; padding: 10px; background: #eee; text-align: center;">Element 3</div>
    </div>`,

      Row4: `<div style="display: flex; justify-content: space-between; gap: 10px;">
      <div style="width: 23%; padding: 10px; background: #eee; text-align: center;">1</div>
      <div style="width: 23%; padding: 10px; background: #eee; text-align: center;">2</div>
      <div style="width: 23%; padding: 10px; background: #eee; text-align: center;">3</div>
      <div style="width: 23%; padding: 10px; background: #eee; text-align: center;">4</div>
    </div>`
    };

    return map[key];
  }


  function insertTemplate(e) {
    const tpl = getTemplate(e.target.value);
    if (!tpl) return;

    // Always insert at the end of the editor
    editor.value.focus();
    const sel = window.getSelection();
    const range = document.createRange();
    range.selectNodeContents(editor.value);
    range.collapse(false); // Move to end
    sel.removeAllRanges();
    sel.addRange(range);

    insertHtmlAtCaret(tpl);
    syncHtml();
  }


  function insertHtmlAtCaret(htmlContent) {
    editor.value.focus();
    const sel = window.getSelection();
    if (!sel || sel.rangeCount === 0) {
      return;
    }
    const range = sel.getRangeAt(0);
    range.deleteContents();
    const el = document.createElement('div');
    el.innerHTML = htmlContent;
    const frag = document.createDocumentFragment();
    let node;
    while ((node = el.firstChild)) {
      frag.appendChild(node);
    }
    range.insertNode(frag);
    range.collapse(false);
    sel.removeAllRanges();
    sel.addRange(range);
  }

  function toggleAlignment(direction) {
    editor.value.focus();
    const selection = window.getSelection();
    if (!selection.rangeCount) return;

    const range = selection.getRangeAt(0);
    let element = range.startContainer;

    // Step up to the nearest element
    while (element && element.nodeType !== 1) {
      element = element.parentNode;
    }

    // If no block element found, wrap the selection in a new div
    if (!element || element === editor.value || !['DIV', 'P', 'SECTION', 'ARTICLE'].includes(element.tagName)) {
      const wrapper = document.createElement('div');
      wrapper.setAttribute('style', getAlignStyle(direction));
      range.surroundContents(wrapper);
      syncHtml();
      return;
    }

    // Apply alignment to the existing block element
    const styleMap = {
      left: 'text-align:left;margin-left:0;margin-right:auto;',
      center: 'text-align:center;margin-left:auto;margin-right:auto;',
      right: 'text-align:right;margin-left:auto;margin-right:0;'
    };

    const currentStyle = element.getAttribute('style') || '';
    const newStyle = styleMap[direction];
    const cleaned = currentStyle
      .replace(/text-align\s*:\s*[^;]+;?/gi, '')
      .replace(/margin-left\s*:\s*[^;]+;?/gi, '')
      .replace(/margin-right\s*:\s*[^;]+;?/gi, '')
      .trim();
    element.setAttribute('style', `${cleaned};${newStyle}`);
    syncHtml();
  }


  function syncHtml() {
    html.value = editor.value.innerHTML;
  }

  function exec(command, value = null) {
    editor.value.focus();
    document.execCommand(command, false, value);
    syncHtml();
  }

  function setFontSize(e) {
    const pxMap = {
      1: '10px',
      2: '13px',
      3: '16px',
      4: '18px',
      5: '24px',
      6: '32px',
      7: '48px'
    };
    const px = pxMap[e.target.value] || '16px';

    const sel = window.getSelection();
    if (!sel.rangeCount) return;

    const range = sel.getRangeAt(0);

    // Clean nested font-size spans inside the selection
    const fragment = range.cloneContents();
    const walker = document.createTreeWalker(fragment, NodeFilter.SHOW_ELEMENT, {
      acceptNode: node =>
        node.nodeType === 1 && node.nodeName === 'SPAN' &&
          node.style.fontSize
          ? NodeFilter.FILTER_ACCEPT
          : NodeFilter.FILTER_SKIP
    });

    while (walker.nextNode()) {
      walker.currentNode.style.fontSize = '';
    }

    // Remove empty styles
    [...fragment.querySelectorAll('span')].forEach(span => {
      if (!span.getAttribute('style')) span.removeAttribute('style');
    });

    // Delete contents and insert wrapped version
    const newSpan = document.createElement('span');
    newSpan.style.fontSize = px;
    newSpan.appendChild(fragment);

    range.deleteContents();
    range.insertNode(newSpan);

    // Reselect after insert
    sel.removeAllRanges();
    const newRange = document.createRange();
    newRange.selectNodeContents(newSpan);
    sel.addRange(newRange);

    syncHtml();
  }


  function getSelectionHtml() {
    const sel = window.getSelection();
    if (!sel || sel.rangeCount === 0) return '';
    const container = document.createElement('div');
    for (let i = 0, len = sel.rangeCount; i < len; ++i) {
      container.appendChild(sel.getRangeAt(i).cloneContents());
    }
    return container.innerHTML;
  }

  function insertCodeBlock() {
    const code = `<pre style="${getAlignStyle('left')}"><code>Your code here</code></pre>`;
    insertHtmlAtCaret(code);
    syncHtml();
  }

  function triggerImageUpload() {
    editor.value.focus();
    imageInput.value.click();
  }

  function handleImageUpload(event) {
    const file = event.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = () => {
      const img = new Image();
      img.onload = () => {
        const canvas = document.createElement('canvas');
        const MAX_WIDTH = 250;
        const scale = MAX_WIDTH / img.width;
        canvas.width = MAX_WIDTH;
        canvas.height = img.height * scale;

        const ctx = canvas.getContext('2d');
        ctx.drawImage(img, 0, 0, canvas.width, canvas.height);

        const resizedBase64 = canvas.toDataURL('image/png');

        // ✅ Focus editor and move caret to the end
        editor.value.focus();
        const sel = window.getSelection();
        const range = document.createRange();
        range.selectNodeContents(editor.value);
        range.collapse(false); // move to end
        sel.removeAllRanges();
        sel.addRange(range);

        insertImage(resizedBase64);
      };
      img.src = reader.result;
    };

    // ❗ You forgot this line:
    reader.readAsDataURL(file);

    // Optional: Reset input for next upload
    event.target.value = '';
  }
  function findMatchingNodeInEditor(clonedNode) {
    const xpath = getElementXPath(clonedNode);
    return getElementByXPath(xpath, editor.value);
  }

  function getElementXPath(el) {
    if (el.id) return `//*[@id="${el.id}"]`;
    if (el === document.body) return '/html/body';

    const ix = Array.from(el.parentNode.children).filter(e => e.tagName === el.tagName).indexOf(el) + 1;
    return `${getElementXPath(el.parentNode)}/${el.tagName.toLowerCase()}[${ix}]`;
  }

  function getElementByXPath(path, contextNode = document) {
    const result = document.evaluate(path, contextNode, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null);
    return result.singleNodeValue;
  }


  function setBackground(e) {
    const color = e.target.value;
    const selection = window.getSelection();
    if (!selection.rangeCount) return;

    const range = selection.getRangeAt(0);
    const commonAncestor = range.commonAncestorContainer;
    const editorEl = editor.value;

    // Get all block elements inside the selection
    const blockTags = ['DIV', 'P', 'SECTION', 'ARTICLE'];
    const walker = document.createTreeWalker(
      commonAncestor.nodeType === 1 ? commonAncestor : commonAncestor.parentNode,
      NodeFilter.SHOW_ELEMENT,
      {
        acceptNode: (node) => {
          // Only accept block-level elements inside the editor and selection
          const tagMatch = blockTags.includes(node.tagName);
          const insideEditor = editorEl.contains(node);
          const overlaps = selectionContainsNode(selection, node);
          return tagMatch && insideEditor && overlaps ? NodeFilter.FILTER_ACCEPT : NodeFilter.FILTER_SKIP;
        }
      }
    );

    const nodes = [];
    while (walker.nextNode()) {
      nodes.push(walker.currentNode);
    }

    // If no matches, fallback to applying on parent block of selection
    if (nodes.length === 0) {
      let node = selection.focusNode;
      if (node.nodeType === 3) node = node.parentElement;

      while (
        node &&
        node !== editorEl &&
        !blockTags.includes(node.tagName)
      ) {
        node = node.parentElement;
      }

      if (node && node !== editorEl) {
        node.style.background = color;
      }

      syncHtml();
      return;
    }

    // Apply background to each matching block element
    for (const node of nodes) {
      const style = node.getAttribute('style') || '';
      const cleaned = style.replace(/background(-color)?\s*:\s*[^;]+;?/gi, '').trim();
      node.setAttribute('style', `${cleaned};background:${color};`);
    }

    syncHtml();
  }


  function selectionContainsNode(selection, node) {
    const range = selection.getRangeAt(0);
    return range.intersectsNode(node);
  }


  function insertImage(base64) {
    const wrapper = document.createElement('div');
    wrapper.contentEditable = 'false';
    wrapper.style.display = 'inline-block';
    wrapper.style.resize = 'both';
    wrapper.style.overflow = 'hidden';
    wrapper.style.maxWidth = '100%';
    wrapper.style.border = '1px dashed #ccc'; // optional visual border

    const img = document.createElement('img');
    img.src = base64;
    img.style.width = '100%';
    img.style.height = 'auto';
    img.draggable = true;

    img.addEventListener('dragstart', (e) => {
      draggedImage.value = wrapper; // 👈 store wrapper, not img
      const ghost = document.createElement('div');
      ghost.style.width = '0px';
      ghost.style.height = '0px';
      document.body.appendChild(ghost);
      e.dataTransfer.setDragImage(ghost, 0, 0);
      setTimeout(() => document.body.removeChild(ghost), 0);
    });

    wrapper.appendChild(img);

    // Insert wrapper
    const sel = window.getSelection();
    if (sel && sel.rangeCount > 0) {
      const range = sel.getRangeAt(0);
      range.deleteContents();
      range.insertNode(wrapper);
      sel.removeAllRanges();
      range.setStartAfter(wrapper);
      range.collapse(true);
      sel.addRange(range);
    }

    syncHtml();
  }





  function clear() {
    editor.value.innerHTML = '';
    syncHtml();
  }

  function loadHtml() {
    editor.value.innerHTML = html.value;
  }

  function downloadHtml() {
    const blob = new Blob([html.value], { type: 'text/html' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = 'email.html';
    link.click();
    URL.revokeObjectURL(link.href);
  }

  function handleHtmlUpload(event) {
    const file = event.target.files[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = (e) => {
      let content = e.target.result;

      // Remove <head> if present
      content = content.replace(/<head[\s\S]*?<\/head>/gi, '');

      // Extract body content if exists
      const bodyMatch = content.match(/<body[^>]*>([\s\S]*?)<\/body>/i);
      if (bodyMatch && bodyMatch[1]) {
        content = bodyMatch[1];
      }

      html.value = content;
      loadHtml();
    };
    reader.readAsText(file);
    event.target.value = '';
  }

  function onInput() {
    syncHtml();
  }

  function handleDrop(event) {
    event.preventDefault();
    const range = document.caretRangeFromPoint(event.clientX, event.clientY);
    if (!range) return;

    const sel = window.getSelection();
    sel.removeAllRanges();
    sel.addRange(range);

    if (draggedImage.value) {
      // 🔒 Stop browser from re-inserting the dragged image
      event.dataTransfer.clearData();

      const img = draggedImage.value;
      draggedImage.value = null;

      // Move the image (not clone)
      range.insertNode(img);
      syncHtml();
      return;
    }

    // External file drop (image from disk)
    const files = event.dataTransfer.files;
    if (files.length && files[0].type.startsWith('image/')) {
      const reader = new FileReader();
      reader.onload = () => insertImage(reader.result);
      reader.readAsDataURL(files[0]);
    }
  }


</script>

<style scoped>
  fieldset {
    width: 100%;
  }
  .editable img {
    max-width: 100%;
    height: auto;
    cursor: move;
    user-select: none;
    resize: both;
    overflow: auto;
    display: inline-block;
  }

  .drag-over {
    border: 2px dashed #007bff;
    background: #f0f8ff;
  }

  .editor-container {
    max-width: 850px;
    margin: auto;
    font-family: Arial, sans-serif;
  }

  /* Horizontal toolbar */
  .toolbar {
    display: flex;
    flex-wrap: wrap;
    gap: 6px;
    margin-bottom: 12px;
    align-items: center;
  }

  /* Buttons smaller and consistent */
  .toolbar-btn {
    padding: 6px 10px;
    font-size: 14px;
    cursor: pointer;
    border-radius: 4px;
    border: none;
    background-color: #222;
    color: white;
    user-select: none;
    transition: background-color 0.3s ease;
    white-space: nowrap;
  }

    .toolbar-btn:hover {
      background-color: #555;
    }

  .toolbar-select {
    padding: 6px 10px;
    font-size: 14px;
    border-radius: 4px;
    border: 1px solid #555;
    background: #222;
    color: white;
    user-select: none;
    min-width: 110px;
  }
  .toolbar-select2 {
    padding: 6px 10px;
    font-size: 16px;
    border-radius: 4px;
    border: 1px solid #555;
    background: #222;
    color: white;
    user-select: none;
    height: 1.89rem;
    width: 2.3rem;
    margin-top: 1rem;
  }
  /* Color input small and aligned */
  .toolbar-color {
    width: 36px;
    height: 28px;
    padding: 0;
    border-radius: 4px;
    border: 1px solid #555;
    cursor: pointer;
    background-color: white;
  }

  /* File upload label as button */
  .file-upload {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    user-select: none;
  }

  /* Editable area styling */
  .editable {
    border: 1px solid #ccc;
    min-height: 420px;
    padding: 12px;
    background: white;
    border-radius: 6px;
    overflow-y: auto;
    font-size: 16px;
    line-height: 1.5;
    color: #111;
    margin-bottom: 20px;
  }

  /* Raw HTML output textarea */
  textarea.raw-html {
    width: 100%;
    min-height: 120px;
    font-family: monospace;
    padding: 12px;
    border-radius: 6px;
    border: 1px solid #ccc;
    resize: vertical;
    font-size: 14px;
  }

  /* Reset margin/padding on inserted blocks for better alignment */
  .editable div,
  .editable p,
  .editable h1,
  .editable h2,
  .editable h3,
  .editable pre {
    margin: 0 0 10px 0;
    padding: 0;
  }
</style>
