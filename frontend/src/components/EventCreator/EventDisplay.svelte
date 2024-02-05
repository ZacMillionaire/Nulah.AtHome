<script lang="ts">
	import type { BasicEvent } from './models/BasicEvent';

	export let Event: BasicEvent;
</script>

<style lang="scss">
  .event {
    border: 1px solid #c2c2c2;
    border-radius: 5px;
    padding: 5px;
		margin: 5px;
    min-width: var(--column-width);
  }
	h1 {
		padding: 0;
		margin: 0;
	}
</style>

<div class="event">
	<h1>{Event.Description}</h1>
	<span>Start: {Event.Start.toLocaleString()}</span>
	{#if Event.End}
		<span>End: {Event.End?.toLocaleString()}</span>
	{/if}
</div>