<template>
  <div class="card-grid">
    <button
      v-for="cardId in cardIds"
      :key="cardId"
      class="card-btn"
      :class="{
        selected: selectedCard === cardId,
        disabled: disabled
      }"
      :disabled="disabled"
      @click="selectCard(cardId)"
    >
      {{ cardId }}
    </button>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  cardCount: {
    type: Number,
    default: 6
  },
  disabled: {
    type: Boolean,
    default: false
  },
  modelValue: {
    type: Number,
    default: null
  }
})

const emit = defineEmits(['update:modelValue', 'select'])

const selectedCard = ref(props.modelValue)
const cardIds = ref([])

watch(() => props.cardCount, (newCount) => {
  cardIds.value = Array.from({ length: newCount }, (_, i) => i + 1)
}, { immediate: true })

watch(() => props.modelValue, (val) => {
  selectedCard.value = val
})

function selectCard(cardId) {
  if (props.disabled) return
  selectedCard.value = cardId
  emit('update:modelValue', cardId)
  emit('select', cardId)
}
</script>
