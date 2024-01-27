<script lang="ts">
	import EventDisplay from './EventDisplay.svelte';
	import EventCreator from './EventCreator.svelte';
	import { EventStore } from './EventStore';
	import { onMount } from 'svelte';

	onMount(() => {
	});
</script>
<EventCreator />
{#each $EventStore as event1}
	<EventDisplay Event={event1} />
{/each}