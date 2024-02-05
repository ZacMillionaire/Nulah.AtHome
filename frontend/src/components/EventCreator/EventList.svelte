<script lang="ts">
	import EventDisplay from './EventDisplay.svelte';
	import { EventStore } from './EventStore';
	import { onMount } from 'svelte';

	onMount(() => {
	});
</script>
<style lang="scss">
  .event-list {
    border: 1px solid #c2c2c2;
    border-radius: 5px;
    margin: 5px;
    min-width: var(--column-width);
    box-shadow: 0px 0px 5px 0px rgba(0, 0, 0, 0.15);
  }
</style>
<div class="event-list">
	{#each $EventStore as event1}
		<EventDisplay Event={event1} />
	{/each}
</div>
