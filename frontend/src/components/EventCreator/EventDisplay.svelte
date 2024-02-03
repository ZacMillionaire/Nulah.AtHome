<script lang="ts">
	import type { BasicEvent } from './models/BasicEvent';

	export let Event: BasicEvent;
</script>

<div>
	<h1>{Event.Description}</h1>
	<span>Start: {new Date(Event.Start).toLocaleString()}</span>
	{#if Event.End}
		<span>End: {new Date(Event.End)?.toLocaleString()}</span>
	{/if}
</div>