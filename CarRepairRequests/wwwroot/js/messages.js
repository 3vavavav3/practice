function showMessage(text, type) {
    const titles = {
        'success': 'Успех',
        'error': 'Ошибка',
        'warning': 'Предупреждение',
        'info': 'Информация'
    };

    const icons = {
        'success': '✅',
        'error': '❌',
        'warning': '⚠️',
        'info': 'ℹ️'
    };

    alert(icons[type] + ' ' + titles[type] + '\n\n' + text);
}

function confirmAction(text) {
    return confirm('❓ Подтверждение\n\n' + text);
}

function checkRequired(ids) {
    for (let id of ids) {
        let el = document.getElementById(id);
        if (!el || !el.value.trim()) {
            showMessage('Поле "' + id + '" обязательно для заполнения', 'warning');
            return false;
        }
    }
    return true;
}