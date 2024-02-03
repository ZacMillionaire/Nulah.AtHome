<script lang="ts">
	export let Errors: string[];
</script>

{#if Errors.length > 0}
	{#each Errors as error}
		<div>{error}</div>
	{/each}
{/if}