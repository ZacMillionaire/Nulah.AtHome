<script lang="ts">
	import type { BasicEvent } from 'models/BasicEvent';

	export let Event: BasicEvent;
</script>

<div>
	<h1>{Event.Name}</h1>
	<span>Start: {Event.Start}</span>
	{#if Event.End}
		<span>End: {Event.End}</span>
	{/if}
</div>