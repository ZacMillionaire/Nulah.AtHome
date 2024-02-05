<script lang="ts">
	export let Errors: string[];
</script>
<style lang="scss">
  .validation-error-container {
    border: 1px solid #f00;
		border-radius: 5px;
		padding: 5px;
		background: #ffc9c9;
  }
</style>
{#if Errors.length > 0}
	<div class="validation-error-container">
		{#each Errors as error}
			<div>{error}</div>
		{/each}
	</div>
{/if}